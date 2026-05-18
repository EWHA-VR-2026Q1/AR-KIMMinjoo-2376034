using UnityEngine;

public class InterfaceBase_IItem : Actor_Grabable, IItem
{
    [Header("Item Info")]
    public ItemData itemData;
    protected Transform itemHolder;

    // 아이템 모드 전용 설정
    public Vector3 ItemUseOffset = new Vector3(0.2f, -0.2f, 0.4f); // 화면 우하단 배치 등

    void Start()
    {
        itemHolder = transform.parent;
    }

    public virtual void OnAdd()
    {
        Debug.Log($"{itemData.Name} 아이템 추가");
    }
    public virtual void OnUse()
    {
        Debug.Log($"{itemData.Name} 메인 기능 실행");
    }

    public virtual void OnStopUse()
    {
        Debug.Log($"{itemData.Name} 사용 중지");
    }
    public virtual void OnEquip(GameObject itemHolder)
    {
        // [추가] itemData가 없는 경우 에러 방지
        if (itemData == null)
        {
            Debug.LogError($"{gameObject.name}의 ItemData가 할당되지 않았습니다!");
            return;
        }

        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(itemHolder.transform);
        transform.localPosition = ItemUseOffset;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"<color=green>[ItemBase]</color> {itemData.Name}이 손에 고정되었습니다");
    }
    /*
    public virtual void OnEquip(GameObject itemHolder) // itemHolder = 장착/고정할 대상
    {
        // 1. 물리 시뮬레이션 완전 정지
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // 2. 부모를 ItemHolder로 설정하고 위치/회전값 리셋
        transform.SetParent(itemHolder.transform);

        // 아이템 전용 위치(ItemUseOffset)로 정렬
        transform.localPosition = ItemUseOffset;
        transform.localRotation = Quaternion.identity;

        Debug.Log($"<color=green>[ItemBsse]</color> {itemData.Name}이 손에 고정되었습니다");
    }
    */

    public virtual void OnUnEquip(GameObject sender) // sender = 대상을 장착/고정하는 Holder
    {
        //if (TryGetComponent<Actor_Grabable>(out var grabActor))
        //{
        //    grabActor.Act_Release();
        //}
        Debug.Log($"<color=green>[ItemBsse]</color> {itemData.Name} 장착 해제");
        Act_Release();        
    }

    public virtual void OnDrop(GameObject sender) // sender = 대상을 장착/고정하는 Holder
    {
        Debug.Log($"<color=green>[ItemBsse]</color> {itemData.Name} 드롭");
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        // 2. 부모를 ItemHolder로 설정하고 위치/회전값 리셋
        transform.SetParent(itemHolder);

        base.Act_Release(sender);
        // 아이템은 보통 플레이어 시야를 가리지 않게 살짝 옆으로 치워줌
        //transform.localPosition = ItemUseOffset;
    }
}