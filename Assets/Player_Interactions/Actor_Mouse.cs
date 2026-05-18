using UnityEngine;

// [RequireComponent(typeof(Rigidbody))]
public class Actor_Mouse : MonoBehaviour
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

    private Transform DefaultParent;
    private Rigidbody rb;

    private void Awake()
    {
        DefaultParent = transform.parent;
        rb = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 게임오브젝트를 쥐고 휘두를 때의 물리 설정
    /// </summary>
    public void Actor_SetGrabPhysics()
    {
        if (rb == null) return;
        rb.drag = 0;           // 잡고 휘두를 때는 저항을 없앰
        rb.angularDrag = 0.05f;
    }

    /// <summary>
    /// 게임오브젝트를 놓을 때, 덜진 때의 물리 설정
    /// </summary>
    public void Actor_SetReleasePhysics()
    {
        if (rb == null) return;
        rb.drag = releaseDrag;               // 서서히 멈추도록 저항 증가
        rb.angularDrag = releaseAngularDrag; // 구르는 것을 방지하기 위해 회전 저항 증가
    }
}