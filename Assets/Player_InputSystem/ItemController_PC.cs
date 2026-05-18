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

    void Awake() => itemBehavior = GetComponent<ItemBehavior>();

    void Update()
    {
        // 1. 장착/드롭 입력 (비헤이비어에게 판단을 맡김)
        if (AddAction.action.WasPressedThisFrame()) itemBehavior.TryAddItem();
        else if (SwapAction.action.WasPressedThisFrame()) itemBehavior.SwitchNextItem();
        // Equip/UnEquip 액션이 눌렸을 때 ToggleEquip을 호출
        else if (EquipAction.action.WasPressedThisFrame() || UnEquipAction.action.WasPressedThisFrame()) 
        {
            itemBehavior.ToggleEquip(); 
        }
        else if (DropAction.action.WasPressedThisFrame()) itemBehavior.TryDrop();
        /*
        if (AddAction.action.WasPressedThisFrame()) itemBehavior.TryAddItem();
        else if (SwapAction.action.WasPressedThisFrame()) itemBehavior.SwitchNextItem();
        else if (EquipAction.action.WasPressedThisFrame()) itemBehavior.TryEquip();
        else if (UnEquipAction.action.WasPressedThisFrame()) itemBehavior.TryUnEquip();
        else if (DropAction.action.WasPressedThisFrame()) itemBehavior.TryDrop();
        */
        // 마우스 휠 체크 (Input System 방식)
        float scrollY = ScrollAction.action?.ReadValue<Vector2>().y ?? 0;
        if (Mathf.Abs(scrollY) > 0.1f) itemBehavior.SwitchNextItem();

        // 사용 로직 (장착 중일 때만)
        if (itemBehavior.IsEquipped)
        {
            if (UseAction.action.WasPressedThisFrame()) itemBehavior.Use();
            if (StopUseAction.action.WasReleasedThisFrame()) itemBehavior.StopUse();
        }
        // 2. 사용 입력
        //if (UseAction.action.WasPressedThisFrame()) itemBehavior.Use();
        //if (StopUseAction.action.WasReleasedThisFrame()) itemBehavior.StopUse();
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