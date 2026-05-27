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