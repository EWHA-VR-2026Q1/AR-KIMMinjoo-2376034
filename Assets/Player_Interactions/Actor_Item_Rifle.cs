using UnityEngine;

public class Actor_Item_Rifle : InterfaceBase_IItem
{
    private bool isFiring = false;
    private float lastFireTime;

    // 1. 발사 시작 (ItemBehavior에서 호출됨)
    public override void OnUse()
    {
        isFiring = true;
    }

    // 2. 발사 중단 (아이템을 뗄 때 호출되도록 부모에 정의되어 있어야 함)
    public override void OnStopUse()
    {
        isFiring = false;
    }

    private void Update()
    {
        // 마우스 버튼을 누르고 있는 상태라면
        if (isFiring)
        {
            // 발사 간격 체크 (itemData에 FireRate가 있다고 가정)
            if (Time.time >= lastFireTime + itemData.FireRate)
            {
                Fire();
                lastFireTime = Time.time;
            }
        }
    }

    private void Fire()
    {
        Debug.Log("탕! (라이플 연사)");
        // 실제 발사 로직 (Raycast 등)
    }

    // 장착 해제 시 연사 중단 안전장치
    // protected override void OnDisable()
    // {
    //     base.OnDisable();
    //     _isFiring = false;
    // }
}
/*
public class Actor_Item_Rifle : InterfaceBase_IItem
{
    public override void OnUse()
    {
        Debug.Log("탕!");
    }  
}
*/