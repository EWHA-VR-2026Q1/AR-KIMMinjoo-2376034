using UnityEngine;

public class Actor_Rifle : InterfaceBase_IItem
{
    [Header("Shoot Options")]
    public Transform FirePoint;
    public GameObject Bullet;
    public float BulletSpeed = 100f;
    public float BulletDamage = 5f;

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

    void Fire()
    {
        Debug.Log("탕! (라이플 연사)");
        // 총구의 위치와 회전값을 그대로 가져옵니다 (이미 월드 기준 좌표임)
        Vector3 pos = FirePoint.position;
        Quaternion dir = FirePoint.rotation;

        GameObject bulletClone = Instantiate(Bullet, pos, dir);
        bulletClone.GetComponent<Actor_Bullet>().SetDamage(BulletDamage);
        
        Rigidbody rb = bulletClone.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // 총구가 바라보는 앞방향(forward)으로 발사
            rb.AddForce(FirePoint.forward * BulletSpeed, ForceMode.VelocityChange);
        }

        Destroy(bulletClone, 2f);
    }
}