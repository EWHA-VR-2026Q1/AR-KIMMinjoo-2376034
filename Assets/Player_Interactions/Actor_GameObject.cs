using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_GameObject : MonoBehaviour
{
    [Header("게임오브젝트를 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_SetActive(): 게임오브젝트 활성화.")]
    [Header("Act_SetActive(bool): 게임오브젝트 활성화. 체크 여부에 따름.")]
    [Header("Act_SetActiveAll(): 게임오브젝트 자식들(손주)까지 활성화. 체크 여부에 따름.")]
    [Header("Act_SetActiveToggle(): 게임오브젝트 활성화-비활성화 토글.")]
    [Header("Act_Destroy(): 게임오브젝트 파괴(소멸).")]
    [Header("Act_Destroy(float): float 만큼 시간(초) 경과 후 게임오브젝트 파괴(소멸).")]
    [Header("Act_QuitGame(): 유니티 에디터 종료.")]
    [Header("--------")]

    [Tooltip("제어할 게임오브젝트 할당. 미할당시 자기 자신을 대상으로 함")]
    public GameObject Target;
    [Tooltip("제어할 게임오브젝트의 활성화 여부. 체크시 활성화 함.")]
    public bool setActive = true;


    private void Awake()
    {
        if (Target == null) Target = gameObject;
    }


    public void Act_SetActive()
    {
        Target.SetActive(setActive);
    }

    public void Act_SetActive(bool active)
    {
        setActive = active;
        Target.SetActive(setActive);
    }

    public void Act_SetActiveAll()
    {
        Debug.Log("SetActiveAll");
        setActive = true;
        Target.SetActive(setActive);

        // true인 인자값은 비활성화된 객체도 포함해서 가져오겠다는 의미
        Transform[] allChildren = Target.GetComponentsInChildren<Transform>(true);

        foreach (Transform child in allChildren)
        {
            // GetComponentsInChildren은 자기 자신(Target)도 포함하므로 
            // 자신을 제외하고 싶다면 조건문을 추가하세요.
            if (child != Target.transform)
            {
                child.gameObject.SetActive(setActive);
            }
        }
    }

    public void Act_SetActiveToggle()
    {
        if (Target.activeSelf == true)
        {
            Target.SetActive(false);
        }
        else
        {
            Target.SetActive(true);
        }
    }

    public void Act_Destroy()
    {
        Destroy(Target);
    }

    public void Act_Destroy(float interval)
    {
        Destroy(Target, interval);
    }

    public void Act_QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
                // 실제 빌드된 앱에서 실행 중일 때
                Application.Quit();
#endif
    }
}
