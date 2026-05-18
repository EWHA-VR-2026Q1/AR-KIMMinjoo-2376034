using UnityEngine;

// [이동 상태] 현재 어떤 이동 컴포넌트가 주도권을 가졌는가?
public enum PlayerMoveState { Ground, Air, Climb, Teleport }

// [상호작용 상태] 손의 상태가 어떠한가?
public enum PlayerInteractionState { Idle, Grab, Item }

[RequireComponent(typeof(CharacterController))]
public class PlayerManager : MonoBehaviour
{
    // 싱글톤 패턴 (선택 사항: 물체들이 PlayerManager.Instance로 쉽게 접근 가능)
    public static PlayerManager Instance;

    public bool isPC = true;

    [Header("Current States")]
    public PlayerMoveState MoveState = PlayerMoveState.Ground;
    public PlayerInteractionState CurrentInteractionState = PlayerInteractionState.Idle;

    [Header("Core References")]
    // public CharacterController Character;
    // public Transform PlayerBody;
    // public Transform PlayerHeadModel;
    public Transform PlayerCamera;
    // public Transform PlayerCameraPivot;
    public Transform PlayerMainMarker;

    [Header("Core Values")]
    public float Gravity = -9.81f;

    // [Header("Hand References (VR/PC Mapping)")]
    // public Transform PlayerPointingHand;    // 물체가 붙을 위치
    // public Transform PlayerTeleportingHand;    // 물체가 붙을 위치

    [Header("Interaction Data")]
    public GameObject CurrentObject; // 현재 잡고 있는 물체 (없으면 null)

    [Header("Component References")]
    // 다른 Behavior들이 PlayerManager를 통해 서로를 참조할 수 있게 열어둡니다.
    public ContinuousMoveBehavior PlayerMove;
    public TeleportBehavior PlayerTeleport;
    //public ClimbBehavior PlayerClimb;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // CharacterController 자동 할당
        // if (Character == null) Character = GetComponent<CharacterController>();

        // Behavior 자동 할당 (필요 시)
        if (PlayerMove == null) PlayerMove = GetComponent<ContinuousMoveBehavior>();
        if (PlayerTeleport == null) PlayerTeleport = GetComponent<TeleportBehavior>();
        //if (PlayerClimb == null) PlayerClimb = GetComponent<ClimbBehavior>();
    }

    public void SetMoveState(PlayerMoveState newState)
    {
        if (MoveState == newState) return;

        MoveState = newState;
        Debug.Log($"<color=yellow>[MoveState]</color> <b>{newState}</b>");
    }

    public PlayerMoveState GetCurrentMoveState()
    {
        return MoveState;
    }

    public void SetInteractionState(PlayerInteractionState newState)
    {
        if (CurrentInteractionState == newState) return;
        CurrentInteractionState = newState;
        Debug.Log($"<color=yellow>[InteractionState]</color> <b>{newState}</b>");
    }

    public PlayerInteractionState GetInteractionState()
    {
        return CurrentInteractionState;
    }

    public void SetCurrentObject(GameObject obj)
{
    CurrentObject = obj;
    if (CurrentObject != null)
    {
        // 물체를 잡거나 아이템을 들면 상태 변경
        SetInteractionState(PlayerInteractionState.Item); 
    }
    else
    {
        SetInteractionState(PlayerInteractionState.Idle);
    }
}


    //public void ToggleMoveControl(bool enable)
    //{
    //    if (PlayerMove != null) PlayerMove.enabled = enable;
    //    if (PlayerTeleport != null) PlayerTeleport.enabled = enable;
    //}
}