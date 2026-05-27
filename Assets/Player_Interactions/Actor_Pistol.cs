using UnityEngine;

public class Actor_Pistol : InterfaceBase_IItem
{
    [Header("Shoot Options")]
    public Transform FirePoint;
    public GameObject Bullet;
    public float BulletSpeed = 100f;
    public float BulletDamage = 1f;


    public override void OnUse()
    {
        Fire();
    }

    void Fire()
    {
        Debug.Log("탕! (피스톨 단사)");
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