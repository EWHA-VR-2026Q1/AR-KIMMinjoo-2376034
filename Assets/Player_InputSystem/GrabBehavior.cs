using UnityEngine;

public class GrabBehavior : MonoBehaviour
{
    // private IGeneral_IS generalInterface; // 상호작용 인터페이스
    private IGrab grabInput;
    // private Ray_Behavior rayBehavior;
    private PointingBehavior pointingBehavior;

    [Header("Grab Settings")]
    public Transform GrabingHand; // 물체가 붙을 손 위치 (카메라 앞 등)
    public GameObject grabbingObject = null;
    
    // 외부에서 현재 잡기 상태를 확인할 수 있는 프로퍼티
    public bool IsGrabbing => grabbingObject != null;
    // private Transform originalParent = null;

    [Header("Touch Settings")]
    [SerializeField] private GameObject touchingObject = null; // 현재 손에 닿아 있는 물체
    
    void Awake()
    {
        grabInput = GetComponent<IGrab>();
        pointingBehavior = GetComponent<PointingBehavior>();
    }

    void Update()
    {
        // 아이템을 이미 장착해서 손에 들고 있다면 그랩 로직 아예 중단
        if (PlayerManager.Instance.CurrentInteractionState == PlayerInteractionState.Item)
        {
            return;
        }
        HandleGrab();
    }

    void HandleGrab()
    {
        if (IsGrabbing) // Release 처리
        {
            // 현재 상태가 Grab이 아니라면 GrabBehavior는 놓기 처리를 무시함
            if (PlayerManager.Instance.GetInteractionState() != PlayerInteractionState.Grab) return;

            Debug.Log($"<color=cyan>[GrabBehavior]</color> Grabing Grabable.");

            if (grabInput.Release)
            {
                Debug.Log($"<color=cyan>[Grab]</color> <b>grabInput.Release :</b> {grabbingObject.name}");
                if (PlayerManager.Instance.GetInteractionState() == PlayerInteractionState.Grab)
                {
                    Debug.Log($"<color=cyan>[Grab]</color> <b>Released:</b> {grabbingObject.name}");
                    grabbingObject.GetComponent<Actor_Grabable>().Act_Release(GrabingHand.gameObject);
                    grabbingObject = null;

                    PlayerManager.Instance.CurrentObject = grabbingObject;
                    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
                }
            }
            return;
        }

        // 원거리 레이캐스트 확인
        if (grabInput.DistanceGrab)
        {
            Debug.Log($"[GrabBehavior] grabInput.DistanceGrab");
            RaycastHit? hit = pointingBehavior.GetRayHit();
            if (hit.HasValue)
            {
                // [추가] IInteractable 인터페이스가 있는지 먼저 확인
                if (hit.Value.collider.TryGetComponent<IInteractable>(out _))
                {
                    if (hit.Value.collider.TryGetComponent<Actor_Grabable>(out var grabActor))
                    {
                        //ExecuteGrab(grabActor); // 그랩 실행 로직을 별도 메서드로 분리하면 깔끔합니다.
                        grabbingObject = grabActor.gameObject; // 관리용 참조
                        grabActor.Act_DistanceGrab(GrabingHand.gameObject);

                        Debug.Log($"<color=cyan>[Grab]</color> <b>DistanceGrab:</b> {grabbingObject.name}");

                        PlayerManager.Instance.CurrentObject = grabbingObject;
                        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
                    }
                }
            }
            /*
            if (hit.HasValue && hit.Value.collider.TryGetComponent<Actor_Grabable>(out var grabActor))
            {
                grabbingObject = grabActor.gameObject; // 관리용 참조
                grabActor.Act_DistanceGrab(GrabingHand.gameObject);

                Debug.Log($"<color=cyan>[Grab]</color> <b>DistanceGrab:</b> {grabbingObject.name}");

                PlayerManager.Instance.CurrentObject = grabbingObject;
                PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
            }
            */
        }

        if (grabInput.DistancePull)
        {
            Debug.Log($"[GrabBehavior] grabInput.DistancePull");
            RaycastHit? hit = pointingBehavior.GetRayHit();
            if (hit.Value.collider.TryGetComponent<IInteractable>(out _))
            {
                if (hit.Value.collider.TryGetComponent<Actor_Grabable>(out var grabActor))
                {
                    //ExecuteGrab(grabActor); // 그랩 실행 로직을 별도 메서드로 분리하면 깔끔합니다.
                    grabbingObject = grabActor.gameObject; // 관리용 참조
                    grabActor.Act_DistancePull(GrabingHand.gameObject);

                    Debug.Log($"<color=cyan>[Grab]</color> <b>DistancePull:</b> {grabbingObject.name}");

                    PlayerManager.Instance.CurrentObject = grabbingObject;
                    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
                }
            }
            /*
            if (hit.HasValue && hit.Value.collider.TryGetComponent<Actor_Grabable>(out var grabActor))
            {
                grabbingObject = grabActor.gameObject; 
                grabActor.Act_DistancePull(GrabingHand.gameObject);
                Debug.Log($"[GrabBehavior] grabInput.DistancePull{grabbingObject.name}");

                PlayerManager.Instance.CurrentObject = grabbingObject;
                PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
            }
            */
        }

        if (grabInput.DistancePoke)
        {
            Debug.Log($"[GrabBehavior] grabInput.DistancePoke");
            RaycastHit? hit = pointingBehavior.GetRayHit();
            if (hit.Value.collider.TryGetComponent<IInteractable>(out _))
            {
                if (hit.Value.collider.TryGetComponent<Actor_Grabable>(out var grabActor))
                {
                    //ExecuteGrab(grabActor); // 그랩 실행 로직을 별도 메서드로 분리하면 깔끔합니다.
                    grabbingObject = null; 
                    grabActor.Act_DistancePoke(GrabingHand.gameObject);

                    Debug.Log($"<color=cyan>[Grab]</color> <b>DistancePoke:</b> {grabbingObject.name}");

                    PlayerManager.Instance.CurrentObject = grabbingObject;
                    PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
                }
            }
        }

        // 근거리 터치 확인
        if (touchingObject != null && touchingObject.TryGetComponent<Actor_Grabable>(out var touchActor))
        {

            if (grabInput.Grab || grabInput.Pinch)
            {
                grabbingObject = touchingObject.gameObject;
                touchActor.Act_Grab(GrabingHand.gameObject);                

                PlayerManager.Instance.CurrentObject = grabbingObject;
                PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
            }
            else if (grabInput.Poke)
            {
                grabbingObject = null;
                touchActor.Act_DistancePoke(gameObject);                

                PlayerManager.Instance.CurrentObject = grabbingObject;
                PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Idle);
            }
        }
    }

    void UpdateInteractionState()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        // 손에 닿았을 때 Actor_Grabable이 있는 물체만 감지 대상으로 등록
        if (other.TryGetComponent<IInteractable>(out _) && other.TryGetComponent<Actor_Grabable>(out _))
        {
            touchingObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (touchingObject == other.gameObject)
        {
            touchingObject = null;
        }
    }
}