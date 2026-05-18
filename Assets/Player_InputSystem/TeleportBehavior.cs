using UnityEngine;

public class TeleportBehavior : MonoBehaviour
{
    private ITeleportInput input;

    [Header("Teleport Settings")]
    public Transform TeleportingHand;
    public GameObject TeleportMarker;
    public string TeleportTag = "Teleportable";
    public float MaxTeleportDistance = 15f;
    public Vector3 Offset = new Vector3(0.2f, -0.2f, 0.3f); // PC 모드 시 카메라 기준 오프셋
    private bool isTeleportCanceled = false;
    private bool isValidTag = false;

    [Header("Arc Visuals")]
    public LineRenderer ArcRenderer;
    public float ArcLineWidth = 0.05f;
    public int ArcResolution = 30;
    public float ArcVelocity = 10f;
    public float ArcStepTime = 0.1f;
    float gravity = -9.81f;

    void Awake()
    {
        input = GetComponent<ITeleportInput>();

        if (ArcRenderer != null)
        {
            ArcRenderer.startWidth = ArcLineWidth;
            ArcRenderer.endWidth = ArcLineWidth;
            ArcRenderer.useWorldSpace = true;
            ArcRenderer.enabled = false;
        }

        Debug.Log("TeleportBehavior");
    }

    void Update()
    {
        // inputSource가 없거나 PlayerManager가 준비되지 않으면 리턴
        if (input == null || PlayerManager.Instance == null) return;

        HandleTeleport();
    }

    void HandleTeleport()
    {
        if(PlayerManager.Instance.isPC) TeleportingHand.SetPositionAndRotation(PlayerManager.Instance.PlayerCamera.TransformPoint(Offset), PlayerManager.Instance.PlayerCamera.rotation);
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
            if (ArcRenderer != null && ArcRenderer.enabled) StopTeleportVisuals();
        }
    }

    void UpdateTeleportPointer()
    {
        // 발사 지점 결정: PlayerManager에 등록된 TeleportingHand가 있다면 그 위치를, 없다면 본인 위치 사용
        // Transform startTransform = PlayerManager.Instance.PlayerTeleportingHand != null
        //                             ? PlayerManager.Instance.PlayerTeleportingHand
        //                             : transform;
        Transform startTransform = TeleportingHand;
        ArcRenderer.enabled = true;
        ArcRenderer.positionCount = ArcResolution;
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
        ArcRenderer.SetPositions(points);
    }

    void ExecuteTeleport(Vector3 targetPosition)
    {
        // CharacterController character = PlayerManager.Instance.Character;
        CharacterController character = GetComponent<CharacterController>();
        if (character == null) return;

        character.enabled = false;
        // 캐릭터 컨트롤러의 높이를 고려하여 위치 조정
        // PlayerManager.Instance.PlayerBody.position = targetPosition + Vector3.up * (character.height / 2f);
        transform.position = targetPosition + Vector3.up * (character.height / 2f);
        character.enabled = true;

        Debug.Log("<color=green>Teleport Success!</color>");
    }

    void StopTeleportVisuals()
    {
        if (ArcRenderer != null) ArcRenderer.enabled = false;
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