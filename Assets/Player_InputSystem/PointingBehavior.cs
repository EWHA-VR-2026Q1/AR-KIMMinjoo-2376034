using UnityEngine;

public class PointingBehavior : MonoBehaviour
{
    private IPointing input; // 레이 입력 인터페이스 (Trigger, IsPointing 등)

    [Header("Settings")]
    public Transform PointingHand; // 레이 발사 기준점 (PlayerPointingHand 또는 카메라)
    public LayerMask InteractableLayer;
    public float MaxDistance = 20f;
    public float LineWidth = 0.01f;
    public Vector3 Offset = new Vector3(0.2f, -0.2f, 0.3f); // PC 모드 시 카메라 기준 오프셋
    
    // public bool isPCMode = true; // PC 모드 여부 (Offset 적용 여부)

    [Header("Visuals")]
    public GameObject HitPointer; // 충돌 지점에 표시될 포인트
    private LineRenderer lineRenderer;

    private RaycastHit? currentHit;
    private string hitObjectName;
    private IInteractable lastInterface;

    private void Start()
    {
        Debug.Log("PointingBehavior");
        input = GetComponent<IPointing>();

        //if(isPCMode) PointingHand.transform.position = PlayerManager.Instance.PlayerCamera.TransformPoint(Offset);

        lineRenderer = PointingHand.GetComponent<LineRenderer>();

        // LineRenderer 초기 설정
        if (lineRenderer != null)
        {
            //Debug.Log("lineRenderer != null");
            lineRenderer.startWidth = LineWidth;
            lineRenderer.endWidth = LineWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        if (HitPointer != null) HitPointer.SetActive(false);

        if(Offset == Vector3.zero) {
            PlayerManager.Instance.PlayerMainMarker.gameObject.SetActive(true);
            Debug.LogWarning("Offset is zero. Ray may start exactly at camera position, which can cause immediate self-collision.");
        }
        else
        {
            PlayerManager.Instance.PlayerMainMarker.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (input == null) return;
        if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Item)
        {
            // 1. 시각 효과 제거
            HideVisuals();

            // 2. [추가] 조준 중이던 물체가 있었다면 Exit 이벤트 강제 발생 후 초기화
            if (lastInterface != null)
            {
                HandleInterfaceState(null);
            }
            return;
        }

        HandlePointing();
    }

    private void HandlePointing()
    {
        if(PlayerManager.Instance.isPC) PointingHand.SetPositionAndRotation(PlayerManager.Instance.PlayerCamera.TransformPoint(Offset), PlayerManager.Instance.PlayerCamera.rotation);
        
        if (input.IsPressing)                       // 레이 버튼을 누르고 있는 동안 (Pointing 중)
        {
            UpdateAnchorPosition();

            Vector3 startPos = PointingHand.position;
            Vector3 direction = PointingHand.forward;
            currentHit = CastRay(startPos, direction);
            hitObjectName = currentHit.HasValue ? currentHit.Value.collider.gameObject.name : "None";
            // Update에서 걸러지지만 이중 보안
            if (PlayerManager.Instance.GetInteractionState() != PlayerInteractionState.Item)
            {
                DrawLine(startPos, direction, currentHit);
            }
            // if(PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Idle && PlayerManager.Instance.CurrentObject == null){
            //     DrawLine(startPos, direction, currentHit);
            //     // if (PlayerManager.Instance.CurrentObject != null)
            //     // {
            //     //     DrawLine(startPos, direction, currentHit);
            //     // }
            // }
            // else if(PlayerManager.Instance.CurrentObject != null)
            // {
            //     // HideVisuals();
            // }
            //DrawLine(startPos, direction, currentHit);            
            HandleInterfaceState(currentHit);       // 인터페이스 상태 처리 (Enter, Stay, Exit)
            if (input.Pressed && currentHit.HasValue) ExecuteClick(currentHit.Value);       // [Click] 버튼을 누른 프레임에 상호작용 실행
        }        
        else if (input.Released)                    // 레이 버튼을 뗐을 때 (Release)
        {                   
            HandleInterfaceState(null);
            currentHit = null;
            hitObjectName = "None";
            HideVisuals();
        }
    }

    private void HandleInterfaceState(RaycastHit? hit)
    {
        IInteractable currentInterface = null;
        if (hit.HasValue) hit.Value.transform.TryGetComponent(out currentInterface);

        if (currentInterface != lastInterface)  // 인터페이스 변경 감지 (Enter / Exit)
        {
            if (currentInterface != null)
            {
                currentInterface.OnEnter(PointingHand.gameObject);
                Debug.Log($"<color=cyan>[Pointing]</color> <b>Enter:</b> {hitObjectName}");
                if (currentHit.HasValue)
                {
                    GameObject target = currentHit.Value.collider.gameObject;
                }
            }

            if (lastInterface != null)
            {
                lastInterface.OnExit(gameObject);
                Debug.Log($"<color=cyan>[Pointing]</color> <b>Exit:</b> {hitObjectName}");
            }
        }
        // 동일한 인터페이스 위에 머물러 있을 때 (Stay)
        else if (currentInterface != null)
        {
            // Debug.Log("<color=cyan>[Pointing]</color> <b>Stay: </b> {gameObject.name}");
            currentInterface.OnStay(PointingHand.gameObject);
        }

        lastInterface = currentInterface;
    }

    private void ExecuteClick(RaycastHit hit)
    {
        if (hit.transform.TryGetComponent<IInteractable>(out var interactable))
        {
            interactable.OnClick(PointingHand.gameObject);
            Debug.Log($"<color=cyan>[Pointing]</color> <b>Click</b>: {hitObjectName}");
        }
    }

    private void UpdateAnchorPosition()
    {
        // PC 모드처럼 카메라가 기준일 경우 약간의 오프셋을 주어 레이가 눈앞에서 나가는 것처럼 연출
        Transform cam = PlayerManager.Instance.PlayerCamera;
        Transform hand = PointingHand;
    }

    private void UpdateHitPointer(RaycastHit hit)
{
    if (HitPointer == null) return;

    // 아이템 장착 중이면 무조건 비활성화 후 리턴
    // if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Item || PlayerManager.Instance.CurrentObject != null)
    // {
    //     HitPointer.SetActive(false);
    //     return;
    // }
    if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Item)
    {
        HitPointer.SetActive(false);
        return;
    }

    if (hit.transform.GetComponent<IInteractable>() != null)
    {
        HitPointer.SetActive(true);
        HitPointer.transform.position = hit.point + (hit.normal * 0.01f);
        HitPointer.transform.rotation = Quaternion.LookRotation(hit.normal);
    }
    else
    {
        HitPointer.SetActive(false);
    }
}
    /*
    private void UpdateHitPointer(RaycastHit hit)
    {
        if (HitPointer == null) return;

        if (hit.transform.GetComponent<IInteractable>() != null)
        {
            if (PlayerManager.Instance.GetInteractionState() != PlayerInteractionState.Item)
            {
                HitPointer.SetActive(true);
                HitPointer.transform.position = hit.point + (hit.normal * 0.01f);
                HitPointer.transform.rotation = Quaternion.LookRotation(hit.normal);
            }
            //HitPointer.SetActive(true);
            //HitPointer.transform.position = hit.point + (hit.normal * 0.01f);
            //HitPointer.transform.rotation = Quaternion.LookRotation(hit.normal);
        }
        else
        {
            HitPointer.SetActive(false);
        }
    }
    */

    public RaycastHit? GetRayHit()
    {
        // Debug.Log($"GetRayHit Called: IsHitting={IsHitting}, CurrentHit={CurrentHit?.collider.name ?? "None"}");
        Transform rayAnchor = PointingHand;
        //rayAnchor.SetPositionAndRotation(PlayerManager.Instance.PlayerCamera.TransformPoint(Offset), PlayerManager.Instance.PlayerCamera.rotation);
        Vector3 rayStartPos = rayAnchor.position;
        Vector3 rayDirection = rayAnchor.forward;
        return CastRay(rayStartPos, rayDirection);
    }
    // public RaycastHit? GetRayHit() => currentHit;
    private RaycastHit? CastRay(Vector3 start, Vector3 dir)
{
    Ray ray = new Ray(start, dir);
    if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, InteractableLayer))
    {
        // [중요] 아이템 장착 중이 아닐 때만 히트 포인터를 업데이트함
        if (PlayerManager.Instance.GetInteractionState() != PlayerInteractionState.Item)
        {
            UpdateHitPointer(hit);
        }
        return hit;
    }

    if (HitPointer != null) HitPointer.SetActive(false);
    return null;
}
    /*
    private RaycastHit? CastRay(Vector3 start, Vector3 dir)
    {
        Ray ray = new Ray(start, dir);
        if (Physics.Raycast(ray, out RaycastHit hit, MaxDistance, InteractableLayer))
        {
            UpdateHitPointer(hit);
            return hit;
        }

        if (HitPointer != null) HitPointer.SetActive(false);
        return null;
    }
    */

    private void DrawLine(Vector3 start, Vector3 dir, RaycastHit? hit)
    {
        if (lineRenderer == null) return;
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, hit.HasValue ? hit.Value.point : start + (dir * MaxDistance));
    }

    private void HideVisuals()
    {
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (HitPointer != null) HitPointer.SetActive(false);
    }
}