using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class Actor_Grabable : MonoBehaviour
{
    [Header("마우스 클릭으로 게임오브젝트의 쥐기/놓기를 제어하는 클래스.")]
    // [Header("Actor_Mouse를 게임오브젝트에 붙일 때 Rigidbody가 자동으로 추가됨.")]
    [Header("--------")]
    [Header("SetGrabPhysics(): \n(선택) 게임오브젝트를 쥐고 휘두를 때의 물리 설정.")]
    [Header("SetReleasePhysics(): \n(선택) 게임오브젝트를 놓을 때, 덜진 때의 물리 설정.")]
    [Header("--------")]

    [Header("Physics Settings (On Release)")]
    [Range(0, 10)] public float releaseDrag = 2f;         // 놓았을 때 이동 저항
    [Range(0, 10)] public float releaseAngularDrag = 2f;  // 놓았을 때 회전 저항

    [Header("Grab Custom Settings")]
    public bool snapToHand = true;      // 즉시 손 위치로 고정될지 여부
    public float pullSpeed = 10f;       // 끌어당길 때의 속도
    public Vector3 GrabOffset = new Vector3(0f, 0f, 0.5f); // 잡을 때 손과의 상대 위치 (카메라 기준)

    private Transform defaultParent;
    private Rigidbody rb;
    private bool isGrabbed = false;
    private bool isPulling = false;
    public float pokeForce = 5f;       // 던지는 세기
    public float releaseForce = 5f;       // 던지는 세기
    public float upwardForce = 2f;      // 약간 위로 솟구치게 하는 세기

    private void Awake()
    {
        defaultParent = transform.parent;
        rb = GetComponent<Rigidbody>();
    }

    public void Act_DistancePoke(GameObject hand)
    {
        Debug.Log($"Act_DistancePoke");
        if (rb == null || isGrabbed) return;

        // 플레이어로부터 물체 방향으로 힘을 가함
        Vector3 pushDirection = (transform.position - hand.transform.position).normalized;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(pushDirection * pokeForce, ForceMode.Impulse);

        Debug.Log($"{gameObject.name}를 멀리서 찔러 밀어냈습니다.");
        Act_Release();
    }
    public void Act_DistanceGrab(GameObject hand)
    {
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
        if (isGrabbed) return;
        isGrabbed = true;

        // 1. 물리 시뮬레이션 일시 정지 (손에 붙어 있어야 하므로)
        rb.isKinematic = true;
        rb.useGravity = false;

        // 2. 부모 설정 (손의 위치를 따라감)
        transform.SetParent(hand.transform);

        // 3. 상태 알림
        //PlayerManager.Instance.CurrentObject = gameObject;
        //PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Grab);
        Debug.Log($"Act_Grab: {gameObject.name} 잡힘");
    }

    public void Act_DistancePull(GameObject hand)
    {
        if (isGrabbed || isPulling) return;
        StartCoroutine(PullRoutine(hand.transform));
    }

    private System.Collections.IEnumerator PullRoutine(Transform hand)
    {
        isPulling = true;
        rb.isKinematic = true;
        rb.useGravity = false;

        float duration = 0.3f;
        float elapsed = 0;

        // 시작 시점의 위치만 기억합니다.
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;

            // 가속도 커브 적용 (점점 빨라지거나 부드럽게 멈추는 효과)
            float curve = Mathf.SmoothStep(0f, 1f, t);

            // [핵심] 매 프레임 hand의 현재 위치와 방향을 계산에 넣습니다.
            // GrabOffset이 단순한 Vector3(0,0,1)이라면 hand 전방 1m 지점이 됩니다.
            Vector3 targetPos = hand.position + (hand.rotation * GrabOffset);
            Quaternion targetRot = hand.rotation;

            transform.position = Vector3.Lerp(startPos, targetPos, curve);
            transform.rotation = Quaternion.Slerp(startRot, targetRot, curve);

            yield return null;
        }

        // 완전히 도착한 후 Grab 로직 실행
        isPulling = false;
        Act_Grab(hand.gameObject);
    }

    public void Act_Grab(GameObject hand)
    {
        if (isGrabbed) return;
        isGrabbed = true;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.SetParent(hand.transform);
        Vector3 targetPos = hand.transform.position + (hand.transform.rotation * GrabOffset);
        Quaternion targetRot = hand.transform.rotation;
        transform.SetPositionAndRotation(targetPos, targetRot);
    }

    public void Act_Poke(GameObject hand)
    {
        Debug.Log("Actor_Poke");
    }

    public void Act_Pinch(GameObject hand)
    {
        Debug.Log("Actor_Pinch");
    }

    public void Act_Release(GameObject hand) // hand = Holder/Hand
    {
        // Debug.Log("Actor_Grabable: Release");
        if (!isGrabbed) return;

        // 상태 초기화
        Act_Release();

        rb.drag = releaseDrag;
        rb.angularDrag = releaseAngularDrag;
        Vector3 forceDirection = hand.transform.forward * releaseForce + Vector3.up * upwardForce;
        // Impulse 모드는 순간적인 충격량을 가할 때 적합합니다.
        rb.AddForce(forceDirection, ForceMode.Impulse);
        // 회전력 추가 (물체가 회전하면서 날아가게 해서 더 자연스럽게 연출)
        rb.AddTorque(Random.insideUnitSphere * 2f, ForceMode.Impulse);
    }

    public void Act_Release() // hand = Holder/Hand
    {
        // 1. 부모 해제
        transform.SetParent(defaultParent);

        // 2. 물리 시뮬레이션 재개
        rb.isKinematic = false;
        rb.useGravity = true;

        // 3. 상태 초기화
        isGrabbed = false;
        isPulling = false;

        Debug.Log($"Actor_Grabable:{gameObject.name} 놓음");
    }
}