using UnityEngine;

public enum ItemType { Weapon, Flashlight, Camera, Album, Consumable }

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Object/ItemData")]
public class ItemData : ScriptableObject
{
    public string Name;
    public float FireRate = 0.1f; // 발사 간격 (초)
    public ItemType Type;
    public Sprite Icon;
    public GameObject Prefab; // 필요 시 생성용
    public string Description;
}