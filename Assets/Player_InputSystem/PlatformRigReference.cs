using UnityEngine;

public enum PlatformType
{
    PC,
    Quest
}
// 이 스크립트는 그냥 인스펙터 연결용 주소록입니다.
public class PlatformRigReferences : MonoBehaviour
{
    public PlatformType Platform; // PC인지 Quest인지 선택

    [Header("이 릭의 실제 오브젝트들")]
    public Transform MainCamera;
    public Transform PointingHand;
    public Transform TeleportHand;
    public Transform ItemHolder;
    public Transform GrabHolder;
}