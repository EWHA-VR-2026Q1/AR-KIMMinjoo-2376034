using UnityEngine;

public class Actor_Bullet : MonoBehaviour
{
    public GameObject MissEffect, HitEffect;
    private float bulletDamage;    
    private bool isHit = false;  

    private Rigidbody rb;
    private Vector3 lastVelocity; // [추가] 물리 엔진이 속도를 0으로 만들기 전의 속도를 기억할 변수

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // [추가] 충돌하기 직전 프레임까지의 실제 속도를 매 프레임 안전하게 기록합니다.
        if (rb != null && rb.velocity.sqrMagnitude > 0.1f)
        {
            lastVelocity = rb.velocity;
        }
    }

    public void SetDamage(float amount)
    {
        bulletDamage = amount;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (isHit) return;

        if (collision.gameObject.CompareTag("Target"))
        {
            isHit = true;
            Debug.Log("[OnCollisionEnter] Hit Target! Damage: " + bulletDamage);
            // 피스톨/라이플 여부에 따라 적절한 calibTime(초) 전달 (예: 0.01f)
            // ShowEffect(HitEffect);
            ShowEffect(HitEffect, 0.005f);
        }        
        else
        {
            Debug.Log("[OnCollisionEnter] Miss Target! Damage: " + bulletDamage);
            // ShowEffect(MissEffect);
            ShowEffect(MissEffect, 0.005f);
        }
        
        Destroy(gameObject);
    }

    void OnTriggerEnter(Collider other)
    {
        if (isHit) return;

        if (other.CompareTag("Target"))
        {
            isHit = true;
            Debug.Log("[OnTriggerEnter] Hit Target! Damage: " + bulletDamage);
            // ShowEffect(HitEffect);
            ShowEffect(HitEffect, 0.01f);
        }
        else
        {
            Debug.Log("[OnTriggerEnter] Miss Target! Damage: " + bulletDamage);
            // ShowEffect(MissEffect);
            ShowEffect(MissEffect, 0.01f);
        }
        
        Destroy(gameObject);
    }

    /*
    // 속도 비례형 이펙트 생성 함수 (보완 완료)
    
    void ShowEffect(GameObject Effect, float calibTime)
    {
        // [보완] 스크립트가 비활성화되었을 때 lastVelocity가 없을 수 있으므로 안전장치 추가
        if (rb == null) rb = GetComponent<Rigidbody>();
    
        // 현재 충돌해서 0이 되었을지도 모르는 rb.velocity 대신, FixedUpdate에서 기록한 lastVelocity를 사용
        Vector3 bulletSpeed = lastVelocity; 

        // 예외 처리: 만약 기록된 속도마저 거의 없다면 Rigidbody 속도나마 대안으로 사용
        if (bulletSpeed.sqrMagnitude < 0.001f && rb != null)
        {
            bulletSpeed = rb.velocity;
        }

        // 만약 완전히 멈춰있는 상태라면 오프셋 없이 현재 위치에 생성
        Vector3 pos = transform.position;
    
        // 기본 회전값
        Quaternion dir = transform.rotation;

        if (bulletSpeed.sqrMagnitude > 0.001f)
        {
            // 현재 위치 - (직전 탄속 * 시간) = 정확히 속도에 비례한 뒤쪽 위치
            pos = transform.position - (bulletSpeed * calibTime);
        
            // [수정 핵심] 날아가던 방향 그대로 이펙트를 정렬하되, 로컬 오프셋을 적용합니다.
        
            // 1. 먼저 이펙트의 Z축(앞)을 총알 날아가는 방향으로 맞춥니다.
            //Quaternion bulletForwardRotation = Quaternion.LookRotation(bulletSpeed.normalized);

            // 2. [추가] 이펙트 모델링의 실제 방향(보통 Y축)을 총알 뒤쪽으로 꺾어주는 오프셋을 계산합니다.
            // 대부분의 이펙트는 Y축이 위로 뻗으므로, X축을 -90도 회전시켜 Z축(앞) 방향으로 눕힙니다.
            // 만약 뱡향이 여전히 이상하다면 (90f, 0f, 0f)로 바꿔보세요.
            //Quaternion localOffset = Quaternion.Euler(-90f, 0f, 0f); 

            // 3. 월드 회전과 로컬 오프셋을 곱하여 최종 회전값을 만듭니다. (순서 주의: World * Local)
            //dir = bulletForwardRotation;
            //dir = bulletForwardRotation * localOffset;
        }

        GameObject hitEffectClone = Instantiate(Effect, pos, dir);
        Destroy(hitEffectClone, 2f);
    }
    */

    
    void ShowEffect(GameObject Effect, float calibTime)
    {
        // [수정] 현재 충돌해서 0이 되었을지도 모르는 rb.velocity 대신, 우리가 기록한 lastVelocity를 사용합니다.
        Vector3 bulletSpeed = lastVelocity; 

        // 예외 처리: 만약 기록된 속도마저 없다면 현재 Rigidbody 속도나마 대안으로 사용
        if (bulletSpeed.sqrMagnitude < 0.001f && rb != null)
        {
            bulletSpeed = rb.velocity;
        }

        // 만약 완전히 멈춰있는 상태라면 오프셋 없이 현재 위치에 생성
        Vector3 pos = transform.position;
        Quaternion dir = transform.rotation;

        if (bulletSpeed.sqrMagnitude > 0.001f)
        {
            // 현재 위치 - (직전 탄속 * 시간) = 정확히 속도에 비례한 뒤쪽 위치
            pos = transform.position - (bulletSpeed * calibTime);
            // 날아가던 방향 그대로 이펙트 정렬
            dir = Quaternion.LookRotation(bulletSpeed.normalized);
        }

        GameObject hitEffectClone = Instantiate(Effect, pos, dir);
        Destroy(hitEffectClone, 2f);
    }
    
    

    void ShowEffect(GameObject Effect)
    {
        // 만약 완전히 멈춰있는 상태라면 오프셋 없이 현재 위치에 생성
        Vector3 pos = transform.position;
        Quaternion dir = transform.rotation;

        GameObject hitEffectClone = Instantiate(Effect, pos, dir);
        Destroy(hitEffectClone, 2f);
    }}