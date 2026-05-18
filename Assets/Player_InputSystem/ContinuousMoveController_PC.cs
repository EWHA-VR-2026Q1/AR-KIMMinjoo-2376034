using UnityEngine;
using UnityEngine.InputSystem;

public class ContinuousMoveController_PC : MonoBehaviour, IContinuousMoveInput // 인터페이스를 세분화하면 더 좋음
{
    [Header("Move Properties")]
    public InputActionProperty MoveAction;
    public InputActionProperty SprintAction;
    public InputActionProperty JumpAction;
    public InputActionProperty SnapTurnAction;

    // IMoveAndLook 중 이동 관련만 구현 (나머지는 zero/false)
    public Vector2 MoveInput => MoveAction.action?.ReadValue<Vector2>() ?? Vector2.zero;
    public bool SprintInput => SprintAction.action?.IsPressed() ?? false;
    public bool JumpInput => JumpAction.action?.WasPressedThisFrame() ?? false;
    public float SnapTurnInput => SnapTurnAction.action?.ReadValue<float>() ?? 0;

    private void OnEnable() => EnableAllActions(true);
    private void OnDisable() => EnableAllActions(false);

    private void EnableAllActions(bool enable)
    {
        // 모든 액션을 포함한 배열 생성
        InputActionProperty[] allActions =
        {
            MoveAction,
            SprintAction,
            JumpAction,
            SnapTurnAction
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