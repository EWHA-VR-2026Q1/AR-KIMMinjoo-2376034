using UnityEngine;
using UnityEngine.InputSystem;

public class PointingController_PC : MonoBehaviour, IPointing
{
    [Header("Ray Properties")]
    public InputActionProperty PressedAction;
    public InputActionProperty IsPressingAction;
    public InputActionProperty ReleasedAction;

    // 인터페이스 프로퍼티 구현
    public bool Pressed => PressedAction.action?.WasPressedThisFrame() ?? false;
    public bool IsPressing => IsPressingAction.action?.IsPressed() ?? false;
    public bool Released => ReleasedAction.action?.WasReleasedThisFrame() ?? false;
    // public bool RaycastInput => RaycastAction.action?.ReadValue<float>() > 0.5f;
    // public bool RaycastDown => RaycastDownAction.action != null && RaycastDownAction.action.IsPressed();
    // public bool RaycastIsPressed => RaycastIsPressedAction.action != null && RaycastIsPressedAction.action.IsPressed();
    // public bool RaycastUp => RaycastUpAction.action != null && RaycastUpAction.action.IsPressed();

    private void OnEnable()
    {
        //Debug.Log("All Actions Enabled");
        EnableAllActions(true);
        SetCursorState(false);
    }

    private void OnDisable()
    {
        //Debug.Log("All Actions Disabled");
        EnableAllActions(false);
        SetCursorState(true);
    }

    private void EnableAllActions(bool enable)
    {
        // 모든 액션을 포함한 배열 생성
        InputActionProperty[] allActions =
        {
            PressedAction,
            IsPressingAction,
            ReleasedAction
        };

        foreach (var property in allActions)
        {
            if (property.action != null)
            {
                if (enable) property.action.Enable();
                else property.action.Disable();
            }
        }
    }

    public void SetCursorState(bool cutsorState)
    {
        Cursor.lockState = cutsorState ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = cutsorState;
    }
}