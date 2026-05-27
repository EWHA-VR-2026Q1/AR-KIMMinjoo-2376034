using UnityEngine;
using UnityEngine.InputSystem;

public class LookController_PC : MonoBehaviour, ILookInput
{
    public InputActionProperty LookAction;

    public Vector2 LookInput => LookAction.action?.ReadValue<Vector2>() ?? Vector2.zero;


    void Start()
    {
        // [핵심 수정 1] PlayerManager의 새로운 변수와 Enum에 맞춰 수정합니다.
        // 현재 플랫폼이 PC가 아니라면(즉, QuestVR이라면) 이 인풋 컴포넌트 자체를 꺼버립니다.
        if (PlayerManager.Instance.CurrentPlatform != TargetPlatform.PC)
        {
            enabled = false;
            return;
        }
    }
    /*
    void Start()
    {
        if (PlayerManager.Instance.PlatformConfig.CurrentPlatform == PlatformType.Quest) enabled = false;
    }
    */

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