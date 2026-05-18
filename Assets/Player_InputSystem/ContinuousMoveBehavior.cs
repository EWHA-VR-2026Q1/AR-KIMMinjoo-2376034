using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class ContinuousMoveBehavior : MonoBehaviour
{
    private CharacterController character;
    //private IMoveAndLook input;
    private IContinuousMoveInput input;

    [Header("Look Settings")]
    [Tooltip("PlayerLook_PC 할당. 미할당시 현재 오브젝트에서 PlayerLook_PC 컴포넌트를 찾음.")]
    //Transform cameraPivot;
    //Transform headModelPivot;
    // Transform playerTransform;
    public float PlayerRotSpeed = 0.1f;

    [Header("Move Settings")]
    public float WalkSpeed = 1.5f;
    public float RunSpeed = 3.5f;
    public float Friction = 0.9f;
    public float ClimbSpeed = 1.2f;
    float ClimbOffset = 0.1f;
    public float JumpHeight = 0.5f;
    float gravity = -9.81f;
    private Vector3 moveVelocity;

    [Header("SnapTurn Settings")]
    bool canSnapTurn = true;
    float snapTurnThreshold = 0.5f;

    [Header("State Settings")]
    int dummyVar;
    enum playerState { Ground, Air, Climb, Teleport }
    playerState currentMoveState;

    enum climbType { None, Ladder, Cliff }
    climbType currentClimbType = climbType.None;

    void Awake()
    {
        character = GetComponent<CharacterController>();
        //input = GetComponent<IMoveAndLook>();
        input = GetComponent<IContinuousMoveInput>();
    }

    private void Start()
    {
        // playerTransform = PlayerManager.Instance.PlayerBody;
        currentMoveState = playerState.Ground;
        currentClimbType = climbType.None;
    }
    void Update()
    {
        if (input == null || character == null || !character.enabled) return;

        PlayerMoveState currentMoveState = PlayerManager.Instance.GetCurrentMoveState();
        // 회전 및 이동 상태 처리
        if (currentMoveState != PlayerMoveState.Teleport)
        {
            HandleSnapTurn();
            HandleMoveState(currentMoveState);
        }

        character.Move(moveVelocity * Time.deltaTime);
    }

    void HandleMoveState(PlayerMoveState state)
    {
        switch (state)
        {
            case PlayerMoveState.Ground: MoveGround(); break;
            case PlayerMoveState.Air: MoveAir(); break;
            case PlayerMoveState.Climb: MoveVertical(); break;
                // case playerState.Teleport: 삭제 (Update에서 직접 관리)
        }
    }

    void MoveGround()
    {
        // 인터페이스를 통해 값을 가져옴
        Vector2 moveInput = input.MoveInput;
        Vector3 dirInput = transform.forward * moveInput.y + transform.right * moveInput.x;

        float speed = input.SprintInput ? RunSpeed : WalkSpeed;

        if (dirInput.magnitude > 0.1f)
        {
            moveVelocity.x = dirInput.x * speed;
            moveVelocity.z = dirInput.z * speed;
        }
        else
        {
            moveVelocity.x *= Friction;
            moveVelocity.z *= Friction;
        }

        // 점프 처리
        if (input.JumpInput && character.isGrounded)
        {
            Debug.Log("Jump!");
            moveVelocity.y = Mathf.Sqrt(JumpHeight * -2f * gravity);
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Air);
            //currentMoveState = playerState.Air;
        }
        else
        {
            moveVelocity.y = -2f;
        }

        // 낙하 감지 (땅에서 떨어짐)
        if (!character.isGrounded && moveVelocity.y < 0)
        {
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Air);
            //currentMoveState = playerState.Air;
        }
    }

    void MoveAir()
    {
        Vector2 moveInput = input.MoveInput;
        Vector3 dirInput = transform.forward * moveInput.y + transform.right * moveInput.x;
        moveVelocity.x = dirInput.x * WalkSpeed;
        moveVelocity.z = dirInput.z * WalkSpeed;
        moveVelocity.y += gravity * Time.deltaTime;

        if (character.isGrounded) PlayerManager.Instance.SetMoveState(PlayerMoveState.Ground);
        //if (character.isGrounded) currentMoveState = playerState.Ground;
    }

    void MoveVertical()
    {
        Vector2 moveInput = input.MoveInput;
        moveVelocity = Vector3.zero;
        if (currentClimbType == climbType.Ladder)
        {
            moveVelocity.y = moveInput.y * ClimbSpeed;
        }
        else if (currentClimbType == climbType.Cliff)
        {
            moveVelocity = transform.right * moveInput.x * ClimbSpeed + Vector3.up * moveInput.y * ClimbSpeed;
            moveVelocity += transform.forward * ClimbOffset;
        }

        if (input.JumpInput)
        {
            Vector3 jumpDir = -transform.forward + Vector3.up;
            moveVelocity = jumpDir.normalized * Mathf.Sqrt(JumpHeight * -2f * gravity);
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Air);
            //currentMoveState = playerState.Air;
        }
    }

    void HandleSnapTurn()
    {
        float turnInput = input.SnapTurnInput;

        // 0.5 이상 밀었을 때만 회전하도록 문턱치(Threshold) 설정 (조이스틱 대응)
        if (Mathf.Abs(turnInput) > snapTurnThreshold && canSnapTurn)
        {
            float turnAngle = turnInput > 0 ? 45f : -45f;
            transform.Rotate(Vector3.up, turnAngle);
            //Debug.Log($"Snap Turn: {turnAngle}");
            canSnapTurn = false;
        }
        // 스틱을 다시 중앙 근처(0.2 미만)로 놓아야 다음 회전 가능
        else if (Mathf.Abs(turnInput) < 0.2f)
        {
            canSnapTurn = true;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("ClimbableLadder")) { 
            currentClimbType = climbType.Ladder; 
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Climb); }
        if (other.CompareTag("ClimbableCliff")) { 
            currentClimbType = climbType.Cliff; 
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Climb); 
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("ClimbableLadder") || other.CompareTag("ClimbableCliff"))
        {
            currentClimbType = climbType.None;
            PlayerManager.Instance.SetMoveState(PlayerMoveState.Air);
        }
    }
}
