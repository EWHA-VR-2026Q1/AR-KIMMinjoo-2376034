using System;
using UnityEngine;

public class Actor_Mount : MonoBehaviour
{
    [Header("Mount State")]
    private Transform originalParent; 
    public Transform MountPoint;      // 탑승 시 초기 위치
    public Collider VehicleTrigger;   // 차량 내부 영역 (이걸 벗어나면 하차)
    
    private bool IsMounted = false;
    private Animator vehicleAnimator;

    private void Awake()
    {
        vehicleAnimator = GetComponent<Animator>();
        
        if(MountPoint == null) {
            MountPoint = transform;
        }
    }

    // [탑승] - Trigger_Collision의 OnEnterEvent 등에 연결
    public void Act_Mount(GameObject passenger)
    {
        if (IsMounted) return;

        // 애니메이터 설정 (애니메이션과 물리 동기화)
        if (vehicleAnimator != null)
            vehicleAnimator.updateMode = AnimatorUpdateMode.AnimatePhysics;

        // 물리 동기화 활성화 (떨림 방지)
        Physics.autoSyncTransforms = true;

        // 부모 설정 (CharacterController를 잠시 꺼야 위치 이동이 정확함)
        if (passenger.TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
            originalParent = passenger.transform.parent;
            
            passenger.transform.SetParent(this.transform);
            // MountPoint가 있다면 그 위치로, 없다면 현재 위치 유지
            passenger.transform.position = MountPoint.position; 
            
            cc.enabled = true; // 다시 켜서 내부 이동 가능하게 함
        }

        IsMounted = true;
        Debug.Log($"<color=cyan>[Mount]</color> {passenger.name} 탑승 완료. 차량 내 이동 가능.");
    }

    // Trigger_Collision의 OnExitEvent에 연결
    public void Act_Dismount(GameObject passenger)
    {
        if (!IsMounted) return;

        // 부모 해제
        if (passenger.TryGetComponent<CharacterController>(out var cc))
        {
            cc.enabled = false;
            passenger.transform.SetParent(originalParent);
            cc.enabled = true;
        }

        // 설정 복구
        if (vehicleAnimator != null)
            vehicleAnimator.updateMode = AnimatorUpdateMode.Normal;
        
        // 주의: 다른 탑승자가 있을 수 있다면 전역 설정을 끌 때 신중해야 함
        Physics.autoSyncTransforms = false; 

        IsMounted = false;
        Debug.Log($"<color=yellow>[Mount]</color> {passenger.name} 하차 완료.");
    }

    // private void IgnoreVehicleCollision(GameObject passenger, bool ignore)
    // {
    //     var vehicleColliders = GetComponentsInChildren<Collider>();
    //     var passengerCollider = passenger.GetComponent<Collider>();

    //     foreach (var vCol in vehicleColliders)
    //     {
    //         // 내부 영역 감지용 트리거는 무시하면 안 됨
    //         if (vCol.isTrigger) continue;
    //         Physics.IgnoreCollision(vCol, passengerCollider, ignore);
    //     }
    // }
}