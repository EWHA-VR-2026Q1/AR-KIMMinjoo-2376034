using UnityEngine;
using UnityEngine.InputSystem;

public class ItemController_PC : MonoBehaviour
{
    private ItemBehavior itemBehavior;

    [Header("Item Actions")]
    public InputActionProperty AddAction;      // 보통 Q키로 설정
    public InputActionProperty SwapAction;     // 보통 Tab키로 설정
    public InputActionProperty ScrollAction;   // 마우스 휠 (Value / Vector2)
    public InputActionProperty EquipAction;
    public InputActionProperty UnEquipAction;
    public InputActionProperty DropAction;

    public InputActionProperty UseAction;
    public InputActionProperty StopUseAction;

    [Header("VR Stick Settings")]
    [SerializeField][Range(0.1f, 0.9f)] private float stickThreshold = 0.5f; // 썸스틱 감도
    private bool isStickReturned = true; // 스틱 연속 입력 방지 플래그

    void Awake() => itemBehavior = GetComponent<ItemBehavior>();

    void Update()
    {
        // 그랩한 물건을 인벤토리에 집어넣는 입력은 상태와 상관없이 항상 허용
        if (AddAction.action.WasPressedThisFrame())
        {
            itemBehavior.TryAddItem();
            return;
        }

        // 현재 플레이어 상태가 '순수 그랩(Grab)' 상태일 때만 체크
        bool isPureGrabbing = PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Grab;

        if (isPureGrabbing)
        {
            // 순수하게 물건을 쥐고 이동/조작 중인 그랩 상태라면,
            // 실수로 아이템이 튀어나와 그랩 해제가 안 되는 버그를 막기 위해 아이템 조작(장착/스위칭)을 차단합니다.
            // 오직 GrabBehavior의 Release 처리만 수행되도록 유도합니다.
            return;
        }

        // [그랩 상태가 아닐 때 - Idle 또는 Item 상태일 때] 
        // 이제 아이템을 쥐고 있는 상태(Item)에서도 아래 조작들이 정상 작동합니다.

        if (SwapAction.action.WasPressedThisFrame()) itemBehavior.SwitchNextItem();
        else if (EquipAction.action.WasPressedThisFrame()) itemBehavior.ToggleEquip();
        else if (UnEquipAction.action.WasPressedThisFrame()) itemBehavior.ToggleEquip(); // 언이큅 가능!
        else if (DropAction.action.WasPressedThisFrame()) itemBehavior.TryDrop();       // 버리기 가능!

        // 아이템 간 스위칭 (썸스틱 / 마우스 휠)
        HandleItemSwitching();

        // 장착 아이템 사용/중지 로직
        if (itemBehavior.IsEquipped)
        {
            if (UseAction.action.WasPressedThisFrame()) itemBehavior.Use();
            if (StopUseAction.action.WasReleasedThisFrame()) itemBehavior.StopUse();
        }
    }

    /*
    void Update()
    {
        if (AddAction.action.WasPressedThisFrame()) itemBehavior.TryAddItem();
        else if (SwapAction.action.WasPressedThisFrame()) itemBehavior.SwitchNextItem();
        else if (EquipAction.action.WasPressedThisFrame()) itemBehavior.ToggleEquip();
        else if (UnEquipAction.action.WasPressedThisFrame()) itemBehavior.ToggleEquip();
        else if (DropAction.action.WasPressedThisFrame()) itemBehavior.TryDrop();

        HandleItemSwitching();

        // 사용 로직 (장착 중일 때만)
        if (itemBehavior.IsEquipped)
        {
            if (UseAction.action.WasPressedThisFrame()) itemBehavior.Use();
            if (StopUseAction.action.WasReleasedThisFrame()) itemBehavior.StopUse();
        }
    }
    */

    private void HandleItemSwitching()
    {
        Vector2 scrollValue = ScrollAction.action?.ReadValue<Vector2>() ?? Vector2.zero;

        // 1. VR 썸스틱 처리 (X축 움직임 감지)
        // 인스펙터의 Modifier 덕분에 그립을 잡았을 때만 이 값이 유효합니다.
        if (Mathf.Abs(scrollValue.x) > 0.1f)
        {
            // 스틱을 완전히 튕겼다가 다시 중앙 영역으로 돌려놓았는지 체크
            if (Mathf.Abs(scrollValue.x) < 0.2f) isStickReturned = true;

            if (isStickReturned)
            {
                if (scrollValue.x > stickThreshold) // 오른쪽으로 튕김
                {
                    itemBehavior.SwitchNextItem();
                    isStickReturned = false;
                }
                else if (scrollValue.x < -stickThreshold) // 왼쪽으로 튕김
                {
                    itemBehavior.SwitchPreviousItem();
                    isStickReturned = false;
                }
            }
        }
        // 2. PC 마우스 휠 처리 (Y축 움직임 감지)
        else if (Mathf.Abs(scrollValue.y) > 0.1f)
        {
            if (scrollValue.y > 0f) itemBehavior.SwitchNextItem();
            else itemBehavior.SwitchPreviousItem();

            // PC 마우스 휠은 순간적인 입력이므로 스틱 리턴 플래그를 상시 초기화해줍니다.
            isStickReturned = true;
        }
        else
        {
            // 입력이 완전히 없으면 스틱이 중앙으로 돌아온 것으로 판단
            isStickReturned = true;
        }
    }

    private void OnEnable() => EnableAllActions(true);
    private void OnDisable() => EnableAllActions(false);

    private void EnableAllActions(bool enable)
    {
        // 모든 액션을 포함한 배열 생성
        InputActionProperty[] allActions =
        {
            AddAction, 
            SwapAction, ScrollAction,
            EquipAction, UnEquipAction,
            UseAction, StopUseAction, 
            DropAction
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