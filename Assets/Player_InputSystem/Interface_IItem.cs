using UnityEngine;
public interface IItem
{
    void OnAdd();
    void OnUse();      // 아이템 기능 실행 (마우스 좌클릭 등)
    void OnStopUse();   // 사용 중지 (마우스 우클릭 등)
    void OnEquip(GameObject sender);  // 장착 시 실행 (sender는 장착한 객체, 보통 플레이어)
    void OnUnEquip(GameObject sender);  // 장착 해제 시 실행 (sender는 해제한 객체, 보통 플레이어)
    void OnDrop(GameObject sender);
}