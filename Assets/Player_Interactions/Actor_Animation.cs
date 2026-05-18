using UnityEngine;

public class Actor_Animation : MonoBehaviour
{
    [Header("애니메이션을 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_SetInteger(int): \nParam에 입력한 값으로 Parameter 변수를 선택하고 \nint로 설정함")]
    [Header("Act_SetInteger(string, int): \nInterface 컴포넌트에서 사용 불가. \nstring에 값을 입력하여 Parameter의 변수를 선택하고 \n변수의 값을 int 값으로 정함")]
    [Header("--------")]

    [Tooltip("Animator 컴포넌트 할당. 미할당시 현재 게임오브젝트의 Animator를 찾음.")]
    public Animator animator;
    [Tooltip("Animator의 Parameters에 있는 \n하나의 Parameter 이름 입력. \n이벤트(Interfce) 컴포넌트에서 사용 \n미입력시 'State'로 자동 설정")]
    public string stateName = "FullAnimation"; // 애니메이터 내 State 이름
    public int totalFrames = 60; // 전체 클립의 프레임 수
    [Tooltip("Animator의 Parameters에 있는 \n하나의 Parameter 이름 입력. 이벤트(Interfce) 컴포넌트")]
    public string param = "State";
    float defaultSpeed = 1f, currentSpeed;

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 애니메이터의 지정된 파라미터에 정수 할당
    /// </summary>
    /// <param name="value"></param>
    public void Act_SetInteger(int value)
    {
        animator.SetInteger(param, value);
    }

    /// <summary>
    /// 애니메이터에 파라미터와 정수 할당
    /// </summary>
    /// <param name="Param"></param>
    /// <param name="value"></param>
    public void Act_SetInteger(string Param, int value)
    {
        animator.SetInteger(Param, value);
    }

    /// <summary>
    /// 애니메이터의 재생 속도 설정 (1이 기본 속도)
    /// </summary>
    /// <param name="speed"></param>
    
    void Act_SetSpeed(float targetSpeed) 
    {
        Debug.Log($"Setting Animation Speed to: {targetSpeed}");
        // Animator.speed 대신 파라미터를 조절합니다.
        // targetSpeed에 -1.0f을 넣으면 즉시 역재생됩니다.
        animator.SetFloat("AniSpeed", targetSpeed);
        
        // 만약 이미 끝부분(1.0)에서 멈춰있다면 강제로 재생 명령을 다시 줍니다.
        if (targetSpeed < 0)
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime >= 0.95f) 
            {
                animator.Play(stateInfo.fullPathHash, 0, 0.99f);
            }
        }
        else
        {
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.normalizedTime <= 0.05f) 
            {
                animator.Play(stateInfo.fullPathHash, 0, 0.01f);
            }
        }
    }

    public void Act_Event_SetSpeed(float speed)
    {
        Act_SetSpeed(speed);
    }

    /// <summary>
    /// 애니메이터의 재생 속도를 기본값으로 리셋 (1)
    /// </summary>
    public void Act_ResetSpeed()
    {
        currentSpeed = defaultSpeed;
        animator.speed = currentSpeed;
    }

    
    public void Act_JumpToFrame(int targetFrame)
    {
        // 애니메이션 재생 속도를 0으로 설정 (정지 화면처럼 보여주기 위함)
        animator.speed = 0;

        // 0~1 사이의 정규화된 시간 값 계산
        float normalizedTime = (float)targetFrame / totalFrames;

        // 3. 특정 상태의 해당 시간으로 점프
        // 세 번째 인자인 normalizedTime이 핵심입니다.
        animator.Play(stateName, 0, normalizedTime);

        // 4. 즉시 반영을 위해 애니메이터 강제 업데이트
        animator.Update(0f);
        Act_ResetSpeed(); // 재생 속도를 기본값으로 리셋하여 애니메이션이 정상적으로 재생되도록 함
    }
}
