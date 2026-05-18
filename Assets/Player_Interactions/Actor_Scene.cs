using UnityEngine;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene(SceneName);
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
