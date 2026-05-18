using UnityEngine;

public class Actor_Item_WeaponTest : InterfaceBase_IItem
{
    public override void OnUse()
    {
        Debug.Log("탕!");
    }
}