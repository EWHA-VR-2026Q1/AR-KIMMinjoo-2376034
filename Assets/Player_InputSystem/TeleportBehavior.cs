using UnityEngine;

public class TeleportBehavior : MonoBehaviour
{
    private ITeleportInput input;

    [Header("Teleport Settings")]    
    public GameObject TeleportMarker;
    public string TeleportTag = "Teleportable";
    public float MaxTeleportDistance = 15f;
    //public Vector3 Offset = new Vector3(0.2f, -0.2f, 0.3f); // PC 모드 시 카메라 기준 오프셋
    private bool isTeleportCanceled = false;
    private bool isValidTag = false;
    Transform teleportingHand;

    [Header("Arc Visuals")]    
    public float ArcLineWidth = 0.05f;
    public int ArcResolution = 30;
    public float ArcVelocity = 10f;
    public float ArcStepTime = 0.1f;
    float gravity = -9.81f;
    LineRenderer arcRenderer;

    void Awake()
    {
        Debug.Log("TeleportBehavior");
        input = GetComponent<ITeleportInput>();
    }

    private void OnEnable()
    {
        // PlayerManager의 이동 상태 변경 이벤트를 구독합니다.
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnMoveStateChanged += HandleMoveStateChanged;
        }
    }

    private void OnDisable()
    {
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.OnMoveStateChanged -= HandleMoveStateChanged;
        }

        // 스크립트가 비활성화될 때 조준선과 마커도 깔끔하게 청소합니다.
        StopTeleportVisuals();
    }

    private void Start()
    {
        teleportingHand = PlayerManager.Instance.GetTeleportHand();
        arcRenderer = teleportingHand.GetComponent<LineRenderer>();
        if (arcRenderer != null)
        {
            arcRenderer.startWidth = ArcLineWidth;
            arcRenderer.endWidth = ArcLineWidth;
            arcRenderer.useWorldSpace = true;
            arcRenderer.enabled = false;
        }
        if (TeleportMarker != null) TeleportMarker.SetActive(false);
    }


    void Update()
    {
        if (input == null || teleportingHand == null) return;

        // [컴파일 에러 해결] 리팩토링된 PlayerManager의 플랫폼 구문을 적용합니다.
        // PC 환경일 경우에만 매 프레임 마우스 시선 방향과 텔레포트 발사 방향을 일치시킵니다.
        if (PlayerManager.Instance.CurrentPlatform == TargetPlatform.PC)
        {
            teleportingHand.rotation = PlayerManager.Instance.GetCamera().rotation;
        }

        HandleTeleport();
    }

    /*
    void Update()
    {
        // inputSource가 없거나 PlayerManager가 준비되지 않으면 리턴
        if (input == null || PlayerManager.Instance == null) return;

        HandleTeleport();
    }
    */

    /// <summary>
    /// 플레이어 이동 상태 변경에 대응하는 이벤트 핸들러
    /// </summary>
    private void HandleMoveStateChanged(PlayerMoveState newState)
    {
        // 상태가 다른 비헤이비어(예: Climb 등)에 의해 주도권을 뺏기면 텔레포트 비주얼을 강제 종료합니다.
        if (newState != PlayerMoveState.Teleport && newState != PlayerMoveState.Ground)
        {
            StopTeleportVisuals();
        }
    }


    void HandleTeleport()
    {
        // Debug.Log($"[TeleportBehavior] HandleTeleport");
        // 1. 조준 버튼을 누르고 있는 중 (Aiming)
        if (input.TeleportAimingInput)
        {
            // Debug.Log($"[TeleportBehavior] HandleTeleport = TeleportAimingInput");
            // 조준 도중 취소 버튼(예: 우클릭 등)이 입력된 경우
            if (input.TeleportCancelInput)
            {
                isTeleportCanceled = true;
                StopTeleportVisuals();
                PlayerManager.Instance.SetMoveState(PlayerMoveState.Ground);
                return;
            }

            if (!isTeleportCanceled)
            {
                // 현재 이동 상태를 주도적으로 Teleport 상태로 전환하고 궤적을 그립니다.
                PlayerManager.Instance.SetMoveState(PlayerMoveState.Teleport);
                UpdateTeleportPointer();
            }
        }
        // 2. 조준 버튼을 떼서 실행하는 순간 (Execute)
        else if (input.TeleportExecuteInput)
        {
            if (!isTeleportCanceled && isValidTag && TeleportMarker != null && TeleportMarker.activeSelf)
            {
                ExecuteTeleport(TeleportMarker.transform.position);
            }
            ResetTeleportState();
        }
        // 3. 아무 입력도 없을 때
        else
        {
            if (arcRenderer != null && arcRenderer.enabled)
            {
                StopTeleportVisuals();
            }
        }
    }

    /*
    void HandleTeleport()
    {
        if (PlayerManager.Instance.PlatformConfig.CurrentPlatform == PlatformType.PC)
        {
            // PC 모드일 때 실행할 이동/스냅턴 로직 작성
            teleportingHand.rotation = PlayerManager.Instance.GetCamera().rotation;
        }

        // 1. 조준 중 (Aiming)
        if (input.TeleportAimingInput)
        {
            Debug.Log("Teleport Aiming...");
            // 조준 중 취소 버튼이 눌렸는지 확인
            if (input.TeleportCancelInput)
            {
                isTeleportCanceled = true;
                StopTeleportVisuals();
                PlayerManager.Instance.SetMoveState(PlayerMoveState.Ground);
            }

            if (!isTeleportCanceled)
            {
                // 상태 변경 및 시각화 업데이트
                PlayerManager.Instance.SetMoveState(PlayerMoveState.Teleport);
                UpdateTeleportPointer();
            }
        }
        // 2. 실행 (Execute)
        else if (input.TeleportExecuteInput)
        {
            if (!isTeleportCanceled && isValidTag && TeleportMarker != null && TeleportMarker.activeSelf)
            {
                ExecuteTeleport(TeleportMarker.transform.position);
            }
            ResetTeleportState();
        }
        // 3. 아무것도 안 할 때
        else
        {
            if (arcRenderer != null && arcRenderer.enabled) StopTeleportVisuals();
        }
    }
    */


    void UpdateTeleportPointer()
    {
        arcRenderer.enabled = true;
        arcRenderer.positionCount = ArcResolution;
        Vector3[] points = new Vector3[ArcResolution];

        Vector3 startPos = teleportingHand.position;
        Vector3 startVelocity = teleportingHand.forward * ArcVelocity;

        isValidTag = false;

        for (int i = 0; i < ArcResolution; i++)
        {
            float t = i * ArcStepTime;
            // 포물선 물리 공식 적용
            Vector3 currentPoint = startPos + (startVelocity * t) + (0.5f * Vector3.up * gravity * t * t);
            points[i] = currentPoint;

            if (i > 0)
            {
                Vector3 prevPoint = points[i - 1];
                Vector3 dir = currentPoint - prevPoint;

                // 포물선 세그먼트 사이를 레이캐스트로 촘촘히 쪼개어 충돌 검사
                if (Physics.Raycast(prevPoint, dir.normalized, out RaycastHit hit, dir.magnitude))
                {
                    if (hit.collider.CompareTag(TeleportTag))
                    {
                        isValidTag = true;
                        TeleportMarker.SetActive(true);

                        // [안전장치] 마커의 위치를 충돌 바닥 지점에 밀착 (피벗이 발바닥인 일반 마커 기준)
                        TeleportMarker.transform.position = hit.point + Vector3.up * 0.01f;

                        // 충돌 지점 이후의 남은 궤적 포인트들은 모두 충돌점으로 뭉개어 선이 뚫고 나가지 않게 함
                        for (int j = i; j < ArcResolution; j++)
                        {
                            points[j] = hit.point;
                        }
                        break;
                    }
                }
            }
        }

        if (!isValidTag) TeleportMarker.SetActive(false);
        arcRenderer.SetPositions(points);
    }
    /*
    void UpdateTeleportPointer()
    {
        // 발사 지점 결정: PlayerManager에 등록된 TeleportingHand가 있다면 그 위치를, 없다면 본인 위치 사용
        // Transform startTransform = PlayerManager.Instance.PlayerTeleportingHand != null
        //                             ? PlayerManager.Instance.PlayerTeleportingHand
        //                             : transform;
        Transform startTransform = teleportingHand;
        arcRenderer.enabled = true;
        arcRenderer.positionCount = ArcResolution;
        Vector3[] points = new Vector3[ArcResolution];

        Vector3 startPos = startTransform.position;
        Vector3 startVelocity = startTransform.forward * ArcVelocity;

        isValidTag = false;

        for (int i = 0; i < ArcResolution; i++)
        {
            float t = i * ArcStepTime;
            // 포물선 공식: P = P0 + V0*t + 0.5*g*t^2
            Vector3 currentPoint = startPos + (startVelocity * t) + (0.5f * Vector3.up * gravity * t * t);
            points[i] = currentPoint;

            if (i > 0)
            {
                Vector3 prevPoint = points[i - 1];
                Vector3 dir = currentPoint - prevPoint;

                if (Physics.Raycast(prevPoint, dir.normalized, out RaycastHit hit, dir.magnitude))
                {
                    if (hit.collider.CompareTag(TeleportTag))
                    {
                        isValidTag = true;
                        TeleportMarker.SetActive(true);
                        // 바닥에 살짝 띄워서 마커 표시 (Z-Fighting 방지)
                        TeleportMarker.transform.position = hit.point + Vector3.up * 0.05f;

                        // 충돌 지점 이후의 모든 포인트는 충돌 지점으로 고정
                        for (int j = i; j < ArcResolution; j++) points[j] = hit.point;
                        break;
                    }
                }
            }
        }

        if (!isValidTag) TeleportMarker.SetActive(false);
        arcRenderer.SetPositions(points);
    }
    */

    void ExecuteTeleport(Vector3 targetPosition)
    {
        // CharacterController character = PlayerManager.Instance.Character;
        CharacterController character = GetComponent<CharacterController>();
        if (character == null) return;

        character.enabled = false;
        // 캐릭터 컨트롤러의 높이를 고려하여 위치 조정
        //transform.position = targetPosition;
        transform.position = targetPosition + Vector3.up * (character.height / 2f);
        character.enabled = true;

        Debug.Log("<color=green>Teleport Success!</color>");
    }

    void StopTeleportVisuals()
    {
        if (arcRenderer != null) arcRenderer.enabled = false;
        if (TeleportMarker != null) TeleportMarker.SetActive(false);
        isValidTag = false;
    }

    void ResetTeleportState()
    {
        isTeleportCanceled = false;
        StopTeleportVisuals();
        PlayerManager.Instance.SetMoveState(PlayerMoveState.Ground);
    }
}