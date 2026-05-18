using UnityEngine;
using UnityEngine.InputSystem;

public class TeleportController_PC : MonoBehaviour, ITeleportInput
{
    public InputActionProperty AimingAction;
    public InputActionProperty ExecuteAction;
    public InputActionProperty CancelAction;

    public bool TeleportAimingInput => AimingAction.action?.IsPressed() ?? false;
    public bool TeleportExecuteInput => ExecuteAction.action?.WasReleasedThisFrame() ?? false;
    public bool TeleportCancelInput => CancelAction.action?.WasReleasedThisFrame() ?? false;

    private void OnEnable() { AimingAction.action?.Enable(); ExecuteAction.action?.Enable(); CancelAction.action?.Enable(); }
    private void OnDisable() { AimingAction.action?.Disable(); ExecuteAction.action?.Disable(); CancelAction.action?.Disable(); }
}