using UnityEngine;

public class LookBehavior : MonoBehaviour
{
    //private IMoveAndLook input;
    private ILookInput input;

    [Header("Sensitivity Settings")]
    public float MouseSensitivity = 1f;
    public float VerticalClampAngle = 80f; // 위아래로 고개를 숙이거나 들 수 있는 한계치

    private float xRotation = 0f; // 상하 회전 누적 값

    [Header("Player Dummy Objects")]
    public Transform DummyHead; // 머리 모델 (카메라와 함께 상하 회전)

    public Transform DummyHand; // 손 모델 (카메라와 함께 상하 회전)
    
    private void Awake()
    {
        // 인터페이스를 구현한 컨트롤러(MoveAndLook_Controller_PC 등) 참조
        //input = GetComponent<IMoveAndLook>();
        Debug.Log("LookBehavior");
    }

    private void Start()
    {
        // PC 환경이라면 마우스 커서를 화면 중앙에 고정하고 숨김
        input = GetComponent<ILookInput>();
        if (input == null) Debug.LogWarning("NO ILookInput input");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (input == null) return;

        HandleLook();
    }

    private void HandleLook()
    {
        // 인터페이스로부터 Look 데이터 수신 (Mouse Delta 또는 Stick 값)
        Vector2 lookInput = input.LookInput * MouseSensitivity;

        // 1. 상하 회전 (카메라만 피벗을 기준으로 회전)
        xRotation -= lookInput.y;
        xRotation = Mathf.Clamp(xRotation, -VerticalClampAngle, VerticalClampAngle);

        /*
        if (PlayerManager.Instance.PlayerCameraPivot != null)
        {
            PlayerManager.Instance.PlayerCameraPivot.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

            // 머리와 손은 이제 회전된 Pivot을 따라갑니다.
            // localRotation을 사용하거나 아예 Pivot의 자식으로 두는 것이 좋습니다.
            if (PlayerManager.Instance.PlayerHead != null)
                PlayerManager.Instance.PlayerHead.localRotation = PlayerManager.Instance.PlayerCameraPivot.localRotation;

            if (PlayerManager.Instance.PlayerPointingHand != null)
                PlayerManager.Instance.PlayerPointingHand.localRotation = PlayerManager.Instance.PlayerCameraPivot.localRotation;
            if (PlayerManager.Instance.PlayerTeleportingHand != null)
                PlayerManager.Instance.PlayerTeleportingHand.localRotation = PlayerManager.Instance.PlayerCameraPivot.localRotation;
            
        }
        */
        transform.Rotate(Vector3.up * lookInput.x);
        DummyHead.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        DummyHand.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        /*
        // Pivot이 없다면 카메라라도 직접 회전 (기존 로직 유지용)
        if (PlayerManager.Instance.PlayerCamera != null)
        {
            PlayerManager.Instance.PlayerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }

        // 2. 좌우 회전 (플레이어 몸통 전체를 회전)
        if (PlayerManager.Instance.PlayerBody != null)
        {
            PlayerManager.Instance.PlayerBody.Rotate(Vector3.up * lookInput.x);
        }
        */
    }
}