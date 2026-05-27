using UnityEngine;

public class PointingBehavior : MonoBehaviour
{
    private IPointing input; // 레이 입력 인터페이스 (Trigger, IsPointing 등)

    [Header("Settings")]    
    public LayerMask InteractableLayer;
    public float MaxDistance = 20f;
    public float LineWidth = 0.01f;
    Transform pointingHand; // 레이 발사 기준점 (PlayerPointingHand 또는 카메라)


    [Header("Visuals")]
    public GameObject HitPointer; // 충돌 지점에 표시될 포인트
    private LineRenderer lineRenderer;

    private RaycastHit? currentHit;
    private string hitObjectName;
    private IInteractable lastInterface;

    private void Awake()
    {
        Debug.Log("PointingBehavior Initialized.");
        input = GetComponent<IPointing>();
    }

    private void OnEnable()
    {
        if (PlayerManager.Instance != null)   // PlayerManager의 상태 변경 이벤트를 구독
        {
            PlayerManager.Instance.OnInteractionStateChanged += HandlePlayerStateChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnInteractionStateChanged -= HandlePlayerStateChanged;
        }

        // 스크립트가 꺼질(Disable) 때 조준선과 포인터도 깔끔하게 숨기고 상호작용을 탈출함
        CleanUpInteraction();
    }

    private void Start()
    {
        pointingHand = PlayerManager.Instance.GetPointingHand();
        lineRenderer = pointingHand.GetComponent<LineRenderer>();

        if (lineRenderer != null)
        {
            lineRenderer.startWidth = LineWidth;
            lineRenderer.endWidth = LineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        if (HitPointer != null) HitPointer.SetActive(false);

        // [최적화] 시작 시점에 이미 아이템을 들고 있는 상태라면 비각성화 처리
        if (PlayerManager.Instance != null && PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Item)
        {
            CleanUpInteraction();
            enabled = false;
        }
    }

    /// <summary>
    /// 플레이어 상호작용 상태 변화에 대응하는 이벤트 핸들러 (핵심 아키텍처)
    /// </summary>
    private void HandlePlayerStateChanged(PlayerInteractionState newState)
    {
        // 아이템 장착 상태(Item)가 되면 시각 효과와 인터페이스를 먼저 완벽히 정리(CleanUp)한 후 컴포넌트를 끔
        if (newState == PlayerInteractionState.Item)
        {
            CleanUpInteraction(); // 잔상 제거 및 상호작용 강제 종료
            enabled = false;
        }
        // 맨손 상태(Idle)로 돌아오면 포인팅 레이저를 다시 활성화합니다.
        else if (newState == PlayerInteractionState.Idle)
        {
            enabled = true;
        }
    }

    private void Update()
    {
        if (input == null || pointingHand == null) return;

        // PC 환경일 경우에만 매 프레임 마우스 시선 방향과 레이저 방향을 일치시킴
        // (VR이나 핸드 트래킹은 기기 하드웨어 추적에 의해 알아서 회전하므로 처리하지 않음)
        if (PlayerManager.Instance.CurrentPlatform == TargetPlatform.PC)
        {
            pointingHand.rotation = PlayerManager.Instance.GetCamera().rotation;
        }

        HandlePointing();
    }


    private void HandlePointing()
    {
        if (input.IsPressing) // 레이 버튼을 누르고 있는 동안 (Pointing 중)
        {
            Vector3 startPos = pointingHand.position;
            Vector3 direction = pointingHand.forward;
            if(PlayerManager.Instance.CurrentInteractionState != PlayerInteractionState.Idle) {
                lineRenderer.enabled = false;
                HitPointer.SetActive(false);
                return;
            }
            currentHit = CastRay(startPos, direction);
            DrawLine(startPos, direction, currentHit);
            // DrawPointer(currentHit.Value);
            // hitObjectName = currentHit.HasValue ? currentHit.Value.collider.gameObject.name : "None";
            if (currentHit.HasValue)
            {
                hitObjectName = currentHit.Value.collider.gameObject.name;
                DrawPointer(currentHit.Value);
            }
            else
            {
                hitObjectName = "None";
                HitPointer.SetActive(false);
            }

            HandleInterfaceState(currentHit); // 인터페이스 상태 처리 (Enter, Stay, Exit)

            // 버튼을 누른 첫 프레임에 충돌체가 있다면 클릭 실행
            if (input.Pressed && currentHit.HasValue)
            {
                ExecuteClick(currentHit.Value);
            }
        }
        else if (input.Released) // 레이 버튼을 뗐을 때 (Release)
        {
            CleanUpInteraction();
        }
    }


    private void HandleInterfaceState(RaycastHit? hit)
    {
        IInteractable currentInterface = null;
        if (hit.HasValue) hit.Value.transform.TryGetComponent(out currentInterface);

        if (currentInterface != lastInterface) // 인터페이스 변경 감지 (Enter / Exit)
        {
            if (currentInterface != null)
            {
                currentInterface.OnEnter(pointingHand.gameObject);
                Debug.Log($"<color=cyan>[Pointing]</color> <b>Enter:</b> {hitObjectName}");
            }

            if (lastInterface != null)
            {
                lastInterface.OnExit(gameObject);
                Debug.Log($"<color=cyan>[Pointing]</color> <b>Exit:</b> {hitObjectName}");
            }
        }
        else if (currentInterface != null) // 동일한 인터페이스 위에 머물러 있을 때 (Stay)
        {
            currentInterface.OnStay(pointingHand.gameObject);
        }

        lastInterface = currentInterface;
    }


    private void ExecuteClick(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.OnClick(pointingHand.gameObject);
            Debug.Log($"<color=cyan>[Pointing]</color> <b>Click</b>: {hitObjectName}");
        }
    }

    private void DrawLine(Vector3 start, Vector3 dir, RaycastHit? hit)
    {
        if (!enabled || lineRenderer == null) return; // 꺼져있으면 선도 그리지 않음
        if (PlayerManager.Instance.CurrentInteractionState != PlayerInteractionState.Idle)
        {
            lineRenderer.enabled = false;
            return;  
        } 

        if(PlayerManager.Instance.CurrentInteractionState == PlayerInteractionState.Idle){
            lineRenderer.enabled = true;
            lineRenderer.SetPosition(0, start);
            lineRenderer.SetPosition(1, hit.HasValue ? hit.Value.point : start + (dir * MaxDistance));
        }
        else
        {
            lineRenderer.enabled = true;
        }
    }


    private void DrawPointer(RaycastHit? hit)
    {     
        if (!enabled || HitPointer == null) return;
        if (PlayerManager.Instance.CurrentInteractionState != PlayerInteractionState.Idle)
        {
            HitPointer.SetActive(false);
            return;  
        } 

        if (hit.Value.transform.GetComponent<IInteractable>() != null)
        {
            HitPointer.SetActive(true);
            HitPointer.transform.position = hit.Value.point + (hit.Value.normal * 0.01f);
            HitPointer.transform.rotation = Quaternion.LookRotation(hit.Value.normal);
        }
        else
        {
            HitPointer.SetActive(false);
        }        
    }


    public RaycastHit? GetRayHit()
    {
        if (pointingHand == null) return null;

        Vector3 rayStartPos = pointingHand.position;
        Vector3 rayDirection = pointingHand.forward;
        return CastRay(rayStartPos, rayDirection);
    }


    private RaycastHit? CastRay(Vector3 start, Vector3 dir)
    {
        Ray ray = new Ray(start, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, InteractableLayer))
        {
            // if (enabled)
            // {
            //     UpdateHitPointer(hit);
            // }
            return hit;
        }

        // if (enabled && HitPointer != null)
        // {
        //     HitPointer.SetActive(false);
        // }

        //if (HitPointer != null) HitPointer.SetActive(false);
        return null;
    }


    private void HideVisuals()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (HitPointer != null) HitPointer.SetActive(false);
    }

    /// <summary>
    /// 포인팅 상태를 초기화하고 상호작용 출구를 정상화하는 청소 함수
    /// </summary>
    private void CleanUpInteraction()
    {
        if (lastInterface != null)
        {
            lastInterface.OnExit(gameObject);
        }

        lastInterface = null;
        currentHit = null;
        hitObjectName = "None";
        HideVisuals();
    }
}