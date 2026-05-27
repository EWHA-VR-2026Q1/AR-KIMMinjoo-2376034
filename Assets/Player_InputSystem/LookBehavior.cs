using UnityEngine;

public class LookBehavior : MonoBehaviour
{
    //private IMoveAndLook input;
    private ILookInput input;

    [Header("Player Camera")]
    Transform playerCamera;

    [Header("Sensitivity Settings")]
    public float MouseSensitivity = 1f;
    public float VerticalClampAngle = 80f; // 위아래로 고개를 숙이거나 들 수 있는 한계치
    private float xRotation; // 상하 회전 누적 값
    private Quaternion yRotation; // 상하 회전 누적 값

    
    private void Awake()
    {
        Debug.Log("LookBehavior");
    }

    private void Start()
    {
        // [핵심 수정 1] PlayerManager에 새로 정의된 currentPlatform 드롭다운 값을 체크합니다.
        // 현재 플랫폼이 PC가 아니라면(즉, QuestVR이라면) 이 컴포넌트를 즉시 끄고 탈출합니다.
        if (PlayerManager.Instance.CurrentPlatform != TargetPlatform.PC)
        {
            enabled = false;
            return; // 컴포넌트가 꺼졌으므로 아래 마우스 커서 락 로직도 실행되지 않습니다.
        }

        // 여기서부터는 오직 "PC 환경"일 때만 실행되는 구역

        // 입력 컴포넌트 가져오기
        input = GetComponent<ILookInput>();
        if (input == null)
        {
            Debug.LogWarning($"[{gameObject.name}] ILookInput 컴포넌트를 찾을 수 없습니다.");
        }

        // PC 환경이므로 마우스 커서를 화면 중앙에 고정하고 숨김
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // PlayerManager가 동적으로 켜준 PC 카메라 Rig의 카메라 주소를 가져옵니다.
        playerCamera = PlayerManager.Instance.GetCamera();
    }

    /*
    private void Start()
    {
        if (PlayerManager.Instance.PlatformConfig.CurrentPlatform == PlatformType.Quest) enabled = false;

        // PC 환경이라면 마우스 커서를 화면 중앙에 고정하고 숨김
        input = GetComponent<ILookInput>();
        if (input == null) Debug.LogWarning("NO ILookInput input");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        playerCamera = PlayerManager.Instance.GetCamera();
    }
    */

    private void LateUpdate()
    {
        // input이 없거나, 어떤 이유로 컴포넌트가 켜져있어도 안전장치로 리턴
        if (input == null) return;

        // [핵심 수정 2] 매 프레임 "플랫폼이 PC인가?"를 if문으로 검사할 필요가 없어졌습니다.
        // 이미 Start단에서 PC가 아니면 스크립트 자체가 꺼졌기 때문에, 이 LateUpdate는 100% PC일 때만 돕니다.
        HandleLook();
    }

    /*
    private void LateUpdate()
    {
        if (input == null) return;

        if (PlayerManager.Instance.PlatformConfig.CurrentPlatform == PlatformType.PC)
        {
            HandleLook();
        }
    }
    */

    private void HandleLook()
    {
        Vector2 lookInput = input.LookInput * MouseSensitivity;
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -VerticalClampAngle, VerticalClampAngle);
        yRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * lookInput.x); // Rot Y Axis
        playerCamera.localRotation = yRotation; // Rot X Axis
    }
}