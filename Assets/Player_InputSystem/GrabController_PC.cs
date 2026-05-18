using UnityEngine;
using UnityEngine.InputSystem;

public class GrabController_PC : MonoBehaviour, IGrab
{
    [Header("Grab Input Settings")]
    public InputActionProperty DistanceGrabAction;    // 예: Mouse Left Click + p Key
    public InputActionProperty DistancePullAction;    // 예: Mouse Left Click + p Key
    public InputActionProperty DistancePokeAction;    // 예: Mouse Left Click + p Key
    public InputActionProperty PokeAction;    // 예: Mouse Left Click + p Key
    public InputActionProperty PinchAction;   // 예: Mouse Left Click (Hold)
    public InputActionProperty GrabAction;    // 예: Mouse Left Click (Hold)
    public InputActionProperty ReleaseAction; // 예: Key R (또는 Grab 해제 시 자동)

    // IGrab 구현
    public bool DistanceGrab => DistanceGrabAction.action?.WasPressedThisFrame() ?? false;
    // public bool DistanceGrab => DistanceGrabAction.action != null && DistanceGrabAction.action.triggered;
    public bool DistancePull => DistancePullAction.action?.IsPressed() ?? false;
    // public bool Pull => PullAction.action?.IsPressed() ?? false;
    public bool DistancePoke => DistancePokeAction.action?.WasPressedThisFrame() ?? false;
    public bool Poke => PokeAction.action?.WasPressedThisFrame() ?? false;

    // Pinch와 Grab은 PC에서 보통 마우스 왼쪽 클릭 하나로 대응
    public bool Pinch => PinchAction.action?.WasPressedThisFrame() ?? false;
    public bool Grab => GrabAction.action?.WasPressedThisFrame() ?? false;

    public bool Release => ReleaseAction.action != null && ReleaseAction.action.WasReleasedThisFrame();


    private void OnEnable()
    {
        EnableAllActions(true);
    }

    private void OnDisable()
    {
        EnableAllActions(false);
    }

    private void EnableAllActions(bool enable)
    {
        // 모든 액션을 포함한 배열 생성
        InputActionProperty[] allActions =
        {
            DistanceGrabAction, DistancePullAction, DistancePokeAction,
            PokeAction, PinchAction, GrabAction,
            ReleaseAction
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