using UnityEngine;
using UnityEngine.InputSystem;

public class LookController_PC : MonoBehaviour, ILookInput
{
    public InputActionProperty LookAction;

    public Vector2 LookInput => LookAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

    private void OnEnable() => EnableAllActions(true);
    private void OnDisable() => EnableAllActions(false);

    private void EnableAllActions(bool enable)
    {
        // 모든 액션을 포함한 배열 생성
        InputActionProperty[] allActions =
        {
            LookAction
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
}