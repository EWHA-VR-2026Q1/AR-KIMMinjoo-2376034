using UnityEngine;

public class Actor_Item : MonoBehaviour
{
    public string ItemName;
    public Sprite ItemIcon; // UI 연결용 (선택)

    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Act_SetState(GameObject sender)
    {
        PlayerManager.Instance.SetInteractionState(PlayerInteractionState.Item);
        Debug.Log("Actor_SetState");
    }

    // public void Act_UnsetState(GameObject sender)
    // {
    //     PlayerManager.Instance.SetState(InteractionState.Idle);
    //     Debug.Log("Actor_UnsetState");
    // }

    // 아이템의 물리적 상태를 제어하는 최소 기능
    public void Act_SetPhysics(bool enable)
    {
        if (rb != null)
        {
            rb.isKinematic = !enable; // 인벤토리에 들어오면 물리 정지
            rb.useGravity = enable;
        }

        // 인벤토리에 있으면 충돌 무시, 월드에 있으면 충돌 활성
        GetComponent<Collider>().enabled = enable;
    }
}

/*
public class Actor_Item : MonoBehaviour, IGeneral_IS
{
    public string ItemName;
    public Sprite ItemIcon; // 인벤토리 UI용
    private Rigidbody rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // [IGeneral_IS] 인터페이스 구현
    public string GetTargetInfo() => $"아이템: {ItemName}";

    public void OnEnter()
    {
        Debug.Log($"{ItemName} 조준 중...");
    }

    public void OnAction()
    {
        Debug.Log($"{ItemName} 액션 실행 중...");
    }

    public void OnExit()
    {
        Debug.Log($"{ItemName} 조준 해제");
    }

    // [IGeneral_IS] 구현 3: 호출자(sender)를 포함한 상태 변화
    public void OnEnter(GameObject sender)
    {
        Debug.Log($"{sender.name}이(가) {ItemName}을 찾음.");
    }

    public void OnAction(GameObject sender)
    {
        // 조준 중일 때 상호작용 (예: 아이템 획득)
        var rayIS = sender.GetComponent<IRay_IS>();
        // if (rayIS != null && rayIS.ExecuteInput) { ... }
        Debug.Log($"{sender.name}이(가) {ItemName}를 주시함.");
    }

    public void OnExit(GameObject sender)
    {
        Debug.Log($"{sender.name}이(가) {ItemName}에서 시선을 돌림.");
    }

    public virtual void Use()
    {
        Debug.Log($"{ItemName}을 사용했습니다.");
    }

    public virtual void StopUse()
    {
        Debug.Log($"{ItemName}을 사용 중지했습니다.");
    }

    public virtual void Equip()
    {
        Debug.Log($"{ItemName}을 장착했습니다.");
    }

    public virtual void UnEquip()
    {
        Debug.Log($"{ItemName}을 탁찰했습니다.");
    }

    public void OnPickUp()
    {
        Debug.Log($"{ItemName}을 획득했습니다.");
    }

    public void OnDrop(Vector3 dropPosition)
    {
        Debug.Log($"{ItemName}을 버렸습니다.");
        gameObject.SetActive(true);
        transform.position = dropPosition;
        if (rb != null) rb.velocity = Vector3.zero;
    }
}
*/