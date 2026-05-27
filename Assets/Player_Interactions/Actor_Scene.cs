using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class Actor_Scene : MonoBehaviour
{
    [Header("씬을 제어하는 클래스")]
    [Tooltip("전환하고 싶은 대상 씬의 이름을 정확히 적어주세요. (예: Stage_02)")]
    public string SceneName;

    void Awake()
    {
        // 만약 인스펙터에서 이름을 안 적었다면 방어 코드로 현재 씬 이름을 넣어줌
        if (string.IsNullOrWhiteSpace(SceneName))
        {
            SceneName = SceneManager.GetActiveScene().name;
        }
    }

    // 마우스 클릭 이벤트 등에서 호출할 메인 함수
    public void Act_LoadScene()
    {
        if (string.IsNullOrWhiteSpace(SceneName)) return;

        print($"{SceneName}으로 이전을 준비합니다.");

        // 마우스 클릭 연산과 세이브 연산이 충돌하지 않도록 코루틴으로 안전하게 실행
        StartCoroutine(SafeSceneTransitionSequence());
    }

    private IEnumerator SafeSceneTransitionSequence()
    {
        // 떠나기 전 현재 오브젝트들을 싹 수집해서 세이브 파일에 씀
        if (Data_Controller.Instance != null)
        {
            Data_Controller.Instance.SaveBeforeLeaveScene();
        }

        // 물리 연산이나 마우스 클릭 이벤트가 안전하게 마무리되도록 딱 1프레임 쉬어줌
        yield return null;

        // 목적지 씬으로 이동
        SceneManager.LoadScene(SceneName);
    }

    // 인스펙터 Event 등에서 문자열을 직접 인자로 넘겨줄 때 안전장치
    public void Act_LoadScene(string sceneName)
    {
        SceneName = sceneName;
        Act_LoadScene();
    }
}
/*
public class Actor_Scene : MonoBehaviour
{
    [Header("씬을 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_LoadScene(): \nSceneName에 입력한 씬을 로딩함")]
    [Header("Act_LoadScene(string): \nSceneName에 입력한 씬 또는 \nInterface에서 string에 입력한 씬을 로딩함")]
    [Header("Act_LoadScene(Object): \nSceneAsset에 할당한 씬 또는 \nInterface에서 Object에 할당한 씬을 로딩함")]
    [Header("--------")]

    [Tooltip("씬의 이름 기입. Interfce(Event) 컴포넌트에서 설정한 값이 우선시 됨")]
    public string SceneName;
    [Tooltip("씬 에셋 할당. Interfce(Event) 컴포넌트에서 설정한 값이 우선시 됨")]
    public Scene SceneObject;

    void Awake()
    {
        if (string.IsNullOrWhiteSpace(SceneName) && SceneObject == null)
        {
            SceneName = SceneManager.GetActiveScene().name;
            SceneObject = SceneManager.GetActiveScene();
            Debug.Log($"SceneObject={SceneObject.name},SceneName={SceneName}");
        }
    }

    public void Act_LoadScene()
    {
        print($"{SceneName}을 로딩합니다.");

        // 씬을 떠나기 전 수집 및 저장 명령 명시
        if (Data_Controller.Instance != null)
        {
            Data_Controller.Instance.SaveBeforeLeaveScene();
        }

        // SceneManager.LoadScene(SceneName);
        Data_Controller.Instance.RestartSceneSafe(SceneName);
    }

    public void Act_LoadScene(string sceneName)
    {
        SceneName = sceneName;
        Act_LoadScene();
    }

    public void Act_LoadScene(Scene scene)
    {
        SceneName = scene.name;
        Act_LoadScene();
    }
}
*/