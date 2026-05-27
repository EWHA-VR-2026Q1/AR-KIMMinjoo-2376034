using UnityEngine;
using System.Collections.Generic;

public class ItemBehavior : MonoBehaviour
{
    [Header("References")]
    Transform holdingHand;
    public Transform FirePoint;
    // public Vector3 Offset = new Vector3(0.2f, -0.2f, 0.3f); // PC 모드 시 카메라 기준 오프셋
    private InterfaceBase_IItem itemBase = null; //현재 창작하고 있는 아이템
    public bool IsEquipped => itemBase != null;

    // ItemBehavior.cs 에 추가
    [Header("Inventory Settings")]
    public List<InterfaceBase_IItem> inventory = new List<InterfaceBase_IItem>();
    public int maxInventorySlot = 5;
    private int currentSlotIndex = 0; // 현재 선택된 아이템의 번호

    void Start()
    {
        holdingHand = PlayerManager.Instance.GetItemHolder();
        // if (Offset == Vector3.zero) {
        //     FirePoint.gameObject.SetActive(true);
        //     Debug.LogWarning("Offset is zero. Ray may start exactly at camera position, which can cause immediate self-collision.");
        // }
        // else
        // {
        //     FirePoint.gameObject.SetActive(false);
        // }
    }
    public void TryAddItem()
    {
        Debug.Log($"<color=cyan>[ItemBehavior]</color> TryAddItem");
        // 1. 현재 그랩 중인 물체가 있는지 확인
        if (TryGetComponent<GrabBehavior>(out var grab) && grab.IsGrabbing)
        {
            GameObject target = grab.GrabbingObject;
            if (target.TryGetComponent<InterfaceBase_IItem>(out var item) && target.GetComponent<IInteractable>() != null)
            {
                if (!inventory.Contains(item)) // 중복 체크
                {
                    Debug.Log($"<color=cyan>[ItemBehavior]</color> TryAddItem --> AddItem");
                    
                    grab.GrabbingObject = null; // 그랩 해제
                    //grab.IsGrabbing = false; // GrabBehavior의 상태 플래그도 명시적으로 꺼줍니다.
                    AddItem(item);
                }
            }
        }
    }

    private void AddItem(InterfaceBase_IItem item)
    {
        if (item == null) return;
        if (inventory.Count >= maxInventorySlot) return;

        string itemName = (item.itemData != null) ? item.itemData.Name : item.gameObject.name;

        inventory.Add(item);
        item.gameObject.SetActive(false);
        item.transform.SetParent(transform);

        // 맨손 상태 유지를 위한 상태 강제 초기화
        // 새로 추가된 아이템이 첫 번째 아이템이든 아니든, 즉시 꺼내지(Equip) 않습니다.
        if (!IsEquipped)
        {
            // 현재 손에 아무것도 들고 있지 않은 상태(itemBase == null)라면 안심하고 완전히 비워줍니다.
            PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
            //PlayerManager.Instance.SetCurrentObject(null);
            // 만약 PlayerManager.Instance.CurrentObject 프로퍼티가 가속기를 타지 않는다면 아래처럼 직접 대입도 안전합니다.
            PlayerManager.Instance.CurrentObject = null;
        }

        Debug.Log($"<color=cyan>[ItemBehavior]</color> AddItem: {itemName} 추가됨. 현재 개수: {inventory.Count}");
        Debug.Log($"<color=cyan>[ItemBehavior]</color> AddItem --> EquipFromInventory");
        //if (inventory.Count == 1) EquipFromInventory(0);
    }

    public void SwitchNextItem()
    {
        if (inventory.Count == 0) return;

        // 아이템이 1개만 있을 때 스위칭을 누르면, 
        // 무반응으로 둘 수도 있지만 '제자리 재장착'을 해주는 것이 로직상 안전
        if (inventory.Count == 1)
        {
            currentSlotIndex = 0;
            Debug.Log($"<color=cyan>[ItemBehavior]</color> SwitchNextItem: 아이템이 1개뿐이므로 제자리 장착합니다.");
            EquipFromInventory(currentSlotIndex);
            return;
        }

        // 아이템이 2개 이상일 때의 정상 스위칭
        currentSlotIndex = (currentSlotIndex + 1) % inventory.Count;
        Debug.Log($"<color=cyan>[ItemBehavior]</color> SwitchNextItem: 인덱스 {currentSlotIndex}로 교체 시도");
        EquipFromInventory(currentSlotIndex);
    }

    public void SwitchPreviousItem()
    {
        if (inventory.Count == 0) return;

        // 아이템이 1개일 때는 제자리 재장착
        if (inventory.Count == 1)
        {
            currentSlotIndex = 0;
            EquipFromInventory(currentSlotIndex);
            return;
        }

        // 왼쪽으로 돌리기 (-1) 처리 및 언더플로우 방지 역방향 인덱스 공식
        currentSlotIndex = (currentSlotIndex - 1 + inventory.Count) % inventory.Count;
        Debug.Log($"<color=cyan>[ItemBehavior]</color> SwitchPreviousItem: 인덱스 {currentSlotIndex}로 교체 시도");
        EquipFromInventory(currentSlotIndex);
    }


    public void EquipFromInventory(int index)
    {
        if (inventory.Count == 0 || index < 0 || index >= inventory.Count) return;

        // 기존 장착 아이템 가방으로 집어넣기
        if (itemBase != null)
        {
            itemBase.gameObject.SetActive(false);
            itemBase.transform.SetParent(transform);
            itemBase = null; // 참조 초기화
        }

        // 새 아이템 꺼내기
        InterfaceBase_IItem nextItem = inventory[index];
        if (nextItem == null) return;

        Debug.Log($"<color=cyan>[ItemBehavior]</color> EquipFromInventory --> Equip {nextItem}");
        currentSlotIndex = index; // 현재 슬롯 인덱스 동기화
        Equip(nextItem);

        string itemName = (nextItem.itemData != null) ? nextItem.itemData.Name : nextItem.gameObject.name;
        Debug.Log($"<color=cyan>[ItemBehavior]</color> EquipFromInventory: {itemName} 장착 완료.");
    }

    public void TryEquip()
    {
        // [추가된 안전장치] 무언가를 그랩 중이거나 손에 들고 있다면 아이템 장착 불가
        if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Grab ||
            PlayerManager.Instance.CurrentObject != null)
        {
            Debug.Log("<color=yellow>[ItemBehavior]</color> 무언가를 잡고 있는 상태(Grab)에서는 아이템을 꺼낼 수 없습니다.");
            return;
        }

        if (IsEquipped && itemBase != null) return;

        if (inventory.Count > 0)
        {
            currentSlotIndex = Mathf.Clamp(currentSlotIndex, 0, inventory.Count - 1);
            EquipFromInventory(currentSlotIndex);
        }
    }
    /*
    public void TryEquip()
    {
        if (IsEquipped && itemBase != null) return;

        if (inventory.Count > 0)
        {
            // 인덱스가 리스트 범위를 벗어나지 않도록 안전장치 후 장착
            currentSlotIndex = Mathf.Clamp(currentSlotIndex, 0, inventory.Count - 1);
            EquipFromInventory(currentSlotIndex);
        }
    }
    */

    public void ToggleEquip()
    {
        if (IsEquipped)
        {
            UnEquip();
        }
        else
        {
            // [추가된 안전장치] 꺼내려고 할 때(가방에서 추출) 그랩 중이면 차단
            if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Grab ||
                PlayerManager.Instance.CurrentObject != null)
            {
                Debug.Log("<color=yellow>[ItemBehavior]</color> 무언가를 잡고 있는 상태에서는 아이템을 꺼낼 수 없습니다.");
                return;
            }

            if (inventory.Count > 0)
            {
                currentSlotIndex = Mathf.Clamp(currentSlotIndex, 0, inventory.Count - 1);
                EquipFromInventory(currentSlotIndex);
            }
        }
    }


    // 실제 장착 로직 (내부 보호)
    public void Equip(InterfaceBase_IItem newItem)
    {
        if (newItem == null) return;

        itemBase = newItem;

        // 1. 아이템 물리적 배치 및 활성화
        itemBase.transform.SetParent(holdingHand);
        itemBase.transform.localPosition = Vector3.zero;
        itemBase.transform.localRotation = Quaternion.identity;
        itemBase.gameObject.SetActive(true);
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip: itemBase.gameObject = {itemBase.gameObject.name}");
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip --> itemBase.OnEquip");

        itemBase.OnEquip(holdingHand.gameObject);

        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip --> SetCurrentObject");
        PlayerManager.Instance.CurrentObject = itemBase.gameObject;
        //PlayerManager.Instance.SetCurrentObject(itemBase.gameObject);

        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip --> SetInteractionState");
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip: {itemBase.name} 완료. CurrentInteractionState={PlayerManager.Instance.CurrentInteractionState}, CurrentObject={PlayerManager.Instance.CurrentObject.name}");
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Equip --> EquipFromInventory");
    }


    public void UnEquip()
    {
        if (itemBase == null) return;
        Debug.Log($"<color=cyan>[ItemBehavior]</color>UnEquip {holdingHand.gameObject}");
        
        itemBase.OnUnEquip(holdingHand.gameObject);  // Actor_Item에게 ItemHolder 전달 (위치 계산용)
        itemBase.gameObject.SetActive(false);
        itemBase = null;

        // 아이템 해제 시 다시 Idle 상태로 복구
        if (PlayerManager.Instance != null)
        {
            PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
            PlayerManager.Instance.CurrentObject = null;
        }

        Debug.Log($"<color=cyan>[ItemBehavior]</color>UnEquip done. CurrentInteractionState={PlayerManager.Instance.CurrentInteractionState}");
    }

    // [신규] 드롭 시도 로직
    public void TryDrop()
    {
        if (!IsEquipped) return;
        //Drop();
        Debug.Log($"<color=cyan>[ItemBehavior]</color> TryDrop");
        DropFromInventory();

    }

    public void Drop()
    {
        if (itemBase == null) return;
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Drop: {itemBase.itemData.Name} 장착 해제");
        itemBase.OnDrop(holdingHand.gameObject); // Actor_Item에게 ItemHolder 전달 (드롭 위치 계산용)

        itemBase = null;
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
        PlayerManager.Instance.CurrentObject = null;
    }

    public void DropFromInventory()
    {
        if (!IsEquipped || itemBase == null) return;

        InterfaceBase_IItem itemToDrop = itemBase;
        inventory.Remove(itemToDrop);

        Drop(); // 부모 해제 및 물리 On

        // [중요 수정] 아이템을 버린 후 인벤토리 인덱스 재조정
        if (inventory.Count > 0)
        {
            // 범위를 벗어나지 않도록 나머지 연산 처리
            currentSlotIndex %= inventory.Count;
            // 자동 장착을 원하시면 아래 주석을 해제하세요.
            // EquipFromInventory(currentSlotIndex);
        }
        else
        {
            currentSlotIndex = 0;
        }
    }

    public void Use() => itemBase?.OnUse();
    public void StopUse() => itemBase?.OnStopUse();


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
        Debug.Log($"<color=cyan>[ItemBehavior]</color> Removing {itemIndex} from Inventory.");
    }
}