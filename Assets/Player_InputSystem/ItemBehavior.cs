using UnityEngine;
using System.Collections.Generic;

public class ItemBehavior : MonoBehaviour
{
    [Header("References")]
    public Transform ItemHolder;
    private InterfaceBase_IItem itemBase = null;
    public bool IsEquipped => itemBase != null;

    // ItemBehavior.cs 에 추가
    [Header("Inventory Settings")]
    public List<InterfaceBase_IItem> inventory = new List<InterfaceBase_IItem>();
    public int maxInventorySlot = 5;
    private int currentSlotIndex = 0; // 현재 선택된 아이템의 번호

    public void TryAddItem()
    {
        // 1. 현재 그랩 중인 물체가 있는지 확인
        if (TryGetComponent<GrabBehavior>(out var grab) && grab.IsGrabbing)
        {
            GameObject target = grab.grabbingObject;
            if (target.TryGetComponent<InterfaceBase_IItem>(out var item) && target.GetComponent<IInteractable>() != null)
            {
                if (!inventory.Contains(item)) // 중복 체크
                {
                    AddItem(item);
                    grab.grabbingObject = null; // 그랩 해제
                    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
                }
            }
        }
    }

    private void AddItem(InterfaceBase_IItem item)
    {
        if (item == null) return;
        if (inventory.Count >= maxInventorySlot) return;

        // itemData가 null인지 미리 확인
        string itemName = (item.itemData != null) ? item.itemData.Name : item.gameObject.name;

        inventory.Add(item);
        item.gameObject.SetActive(false);
        item.transform.SetParent(this.transform);

        Debug.Log($"[Inventory] {itemName} 추가됨. 현재 개수: {inventory.Count}");

        if (inventory.Count == 1) EquipFromInventory(0);
    }
    /*
    private void AddItem(InterfaceBase_IItem item)
    {
        if (inventory.Count >= maxInventorySlot) return;

        inventory.Add(item);
        // 물리 비활성화 및 시야에서 제거
        item.gameObject.SetActive(false);
        item.transform.SetParent(this.transform);

        Debug.Log($"[Inventory] {item.itemData.Name} 추가됨. 현재 개수: {inventory.Count}");

        // 만약 첫 번째 아이템이라면 바로 장착 시도
        if (inventory.Count == 1) EquipFromInventory(0);
    }
    */

    public void SwitchNextItem()
    {
        if (inventory.Count < 2) return; // 교체할 아이템이 없으면 리턴

        currentSlotIndex = (currentSlotIndex + 1) % inventory.Count;
        EquipFromInventory(currentSlotIndex);
    }
// ItemBehavior.cs

public void EquipFromInventory(int index)
{
    if (index < 0 || index >= inventory.Count) return;

    // [중요 수정] 기존 아이템 처리 전 null 체크 추가
    // 맨손 상태(UnEquip)일 때는 itemBase가 null이므로 바로 새 아이템을 꺼내야 합니다.
    if (itemBase != null)
    {
        itemBase.gameObject.SetActive(false);
        itemBase.transform.SetParent(this.transform);
    }

    // 새 아이템 꺼내기
    InterfaceBase_IItem nextItem = inventory[index];
    if (nextItem == null) return; 

    nextItem.gameObject.SetActive(true);

    // 실제 장착 로직 실행
    Equip(nextItem);
    
    // 로그 출력 시에도 데이터 존재 여부 확인
    string itemName = (nextItem.itemData != null) ? nextItem.itemData.Name : nextItem.gameObject.name;
    Debug.Log($"[Inventory] {itemName}으로 교체됨");
}
    /*
    public void EquipFromInventory(int index)
    {
        if (index < 0 || index >= inventory.Count) return;

        // 1. 기존 아이템은 다시 가방(비활성화)으로
        if (itemBase != null)
        {
            itemBase.gameObject.SetActive(false);
            itemBase.transform.SetParent(this.transform);
        }

        // 2. 새 아이템 꺼내기
        InterfaceBase_IItem nextItem = inventory[index];
        nextItem.gameObject.SetActive(true);

        // 3. 기존 Equip 로직 활용 (손에 붙이기)
        Equip(nextItem);
        Debug.Log($"[Inventory] {nextItem.itemData.Name}으로 교체됨");
    }
    */
    // 장착 시도 로직
    public void TryEquip()
    {
        if (IsEquipped) return; // 이미 장착 중이면 무시

        GameObject currentObj = PlayerManager.Instance.CurrentObject;
        if (currentObj == null) return;

        // 장착 가능한 아이템인지 확인
        if (currentObj.TryGetComponent<InterfaceBase_IItem>(out var newItem))
        {
            // 1. 이미 같은 걸 들고 있다면 무시
            if (itemBase == newItem) return;

            Equip(newItem);
        }
    }

    public void TryUnEquip()
    {
        if (!IsEquipped) return; // 장착된 상태가 아니면 해제할 것도 없음
        UnEquip();
    }

    public void ToggleEquip()
    {
        if (IsEquipped)
        {
            // 현재 장착 중이면 인벤토리로 집어넣기 (비활성화)
            UnEquip(); 
        }
        else if (inventory.Count > 0)
        {
            // 맨손 상태이고 인벤토리에 아이템이 있다면, 
            // 마지막으로 사용했던(혹은 currentSlotIndex에 있는) 아이템을 다시 장착
            EquipFromInventory(currentSlotIndex); 
        }
    }

    // [신규] 드롭 시도 로직
    public void TryDrop()
    {
        if (!IsEquipped) return;
        //Drop();
        DropFromInventory();
    }

    // 실제 장착 로직 (내부 보호)
    public void Equip(InterfaceBase_IItem newItem)
{
    if (newItem == null) return;

    itemBase = newItem;
    
    // 1. 아이템 물리적 배치 및 활성화
    itemBase.transform.SetParent(ItemHolder);
    itemBase.transform.localPosition = Vector3.zero;
    itemBase.transform.localRotation = Quaternion.identity;
    itemBase.gameObject.SetActive(true);

    itemBase.OnEquip(ItemHolder.gameObject);

    // 2. [중요] PlayerManager의 상태를 Item으로 변경
    if (PlayerManager.Instance != null)
    {
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
    }
    
    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
    
    Debug.Log($"{itemBase.name} 장착 완료. InteractionState: Item");
}
    /*
 private void Equip(InterfaceBase_IItem item)
{
    if (item == null) return;

    if (TryGetComponent<GrabBehavior>(out var grab))
    {
        grab.grabbingObject = null;
    }

    // OnEquip 내부에서 에러가 나더라도 시스템이 멈추지 않게 itemBase 할당 전후 처리
    item.OnEquip(ItemHolder.gameObject);
    itemBase = item;

    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
    PlayerManager.Instance.CurrentObject = item.gameObject;

    // itemData가 할당 안 된 경우를 위한 예외 처리
    if (item.itemData != null)
    {
        Debug.Log($"[ItemBehavior] {item.itemData.Name} 장착 완료");
    }
}
*/
    /*
    private void Equip(InterfaceBase_IItem item)
    {
        // GrabBehavior와의 충돌 방지
        if (TryGetComponent<GrabBehavior>(out var grab))
        {
            grab.grabbingObject = null;
        }

        item.OnEquip(ItemHolder.gameObject);
        itemBase = item;

        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
        PlayerManager.Instance.CurrentObject = item.gameObject;
        Debug.Log($"[ItemBehavior] {item.itemData.Name} 장착 완료");
    }
    */

public void UnEquip()
{
    if (itemBase == null) return;

    // itemBase.OnUnEquip();
    itemBase.OnUnEquip(ItemHolder.gameObject);  // Actor_Item에게 ItemHolder 전달 (위치 계산용)

    itemBase.gameObject.SetActive(false);
    itemBase = null;

    // 3. [중요] 아이템 해제 시 다시 Idle 상태로 복구
    if (PlayerManager.Instance != null)
    {
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
    }

    Debug.Log("아이템 해제 완료. InteractionState: Idle");
}
/*
    public void UnEquip()
    {
        if (itemBase == null) return;
        Debug.Log($"<color=magenta>[ItemBehavior]</color> UnEquip {PlayerManager.Instance.CurrentObject.name}");

        if (!inventory.Contains(itemBase))
        {
            inventory.Add(itemBase);
        }

        // 2. 아이템 비활성화 (바닥에 버리지 않고 플레이어 자식으로 보관)
        itemBase.gameObject.SetActive(false);
        itemBase.transform.SetParent(this.transform);

        // 3. 상태 초기화 (맨손 상태)
        itemBase = null;
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
        PlayerManager.Instance.CurrentObject = null;

        //itemBase.OnUnEquip(ItemHolder.gameObject);  // Actor_Item에게 ItemHolder 전달 (위치 계산용)

        //UpdateInteractionState(PlayerInteractionState.Idle, null);

        //if (TryGetComponent<Actor_Grabable>(out var grabActor))
        //{
        //    grabActor.Act_Release();
        //}
    }
    */

    public void Use() => itemBase?.OnUse();
    public void StopUse() => itemBase?.OnStopUse();

    public void Drop()
    {
        if (itemBase == null) return;
        Debug.Log($"[Item_Behavior] {itemBase.itemData.Name} 장착 해제");
        itemBase.OnDrop(ItemHolder.gameObject); // Actor_Item에게 ItemHolder 전달 (드롭 위치 계산용)

        itemBase = null;
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
        PlayerManager.Instance.CurrentObject = null;
    }

    public void DropFromInventory()
    {
        if (!IsEquipped) return;

        InterfaceBase_IItem itemToDrop = itemBase;
        inventory.Remove(itemToDrop); // 리스트에서 제거
        Drop(); // 기존 드롭 로직 (물리 켜고 부모 해제)

        // 다음 아이템 자동 장착 (선택 사항)
        if (inventory.Count > 0)
        {
            // 버린 후 남은 아이템이 있다면 첫 번째(혹은 이전 인덱스) 아이템을 다시 장착
            currentSlotIndex %= inventory.Count; // 인덱스 초과 방지
            EquipFromInventory(currentSlotIndex);
        }
    }


    void UpdateInteractionState(PlayerInteractionState state, InterfaceBase_IItem item)
    {
        itemBase = item;
        PlayerManager.Instance.SetInteractionState(state);
        if (itemBase != null)
        {
            PlayerManager.Instance.CurrentObject = itemBase.gameObject;
        }
        else
        {
            PlayerManager.Instance.CurrentObject = null;
        }
    }

    void RemoveFromInventory(int itemIndex)
    {
        Debug.Log($"<color=magenta>[ItemBehavior]</color> Removing {itemIndex} from Inventory.");
    }
}