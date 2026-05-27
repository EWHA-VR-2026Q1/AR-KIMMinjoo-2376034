using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Data_Controller : MonoBehaviour
{
    public static Data_Controller Instance { get; private set; }

    [Header("세이브 러더 참조")]
    public Actor_DataSaverLoader DataActor;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        // X키를 누르면 데이터 초기화 후 씬 재로드
        if (Keyboard.current.xKey.wasPressedThisFrame)
        {
            if (DataActor != null)
            {
                DataActor.Act_ResetAndClearData();
                // 초기화 후 깔끔하게 현재 씬 재시작
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        
        // [테스트용] R키를 누르면 "안전하게 저장 후" 동일 씬 재로드
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            RestartSceneSafe();
        }
    }

    // [핵심] 안전하게 현재 씬을 재시작하는 함수
    public void RestartSceneSafe()
    {
        if (DataActor != null)
        {
            // 1. 오브젝트들이 파괴되기 전, 온전한 상태일 때 먼저 수집하고 저장합니다.
            CollectSaveableEntities(); 
            DataActor.Act_SaveData();
        }

        // 2. 저장이 완전히 끝난 것을 보장한 뒤 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void RestartSceneSafe(string SceneName)
    {
        if (DataActor != null)
        {
            // 1. 오브젝트들이 파괴되기 전, 온전한 상태일 때 먼저 수집하고 저장합니다.
            CollectSaveableEntities(); 
            DataActor.Act_SaveData();
        }

        // 2. 저장이 완전히 끝난 것을 보장한 뒤 씬을 다시 로드합니다.
        SceneManager.LoadScene(SceneName);
    }

    public void SaveBeforeLeaveScene()
    {
        if (DataActor != null)
        {
            CollectSaveableEntities(); // 현재 씬의 오브젝트들을 깨끗하게 다시 수집
            DataActor.Act_SaveData();  // 파일에 쓰기
        }
    }

    private void OnEnable()
    {
        // 불안정한 sceneUnloaded 이벤트 등록을 제거
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene newScene, LoadSceneMode mode)
    {
        StartCoroutine(SafeLoadSequence(newScene.name));
    }

    private IEnumerator SafeLoadSequence(string sceneName)
    {
        // 유니티가 오브젝트를 하이어라키에 완전히 올릴 때까지 1프레임 대기
        yield return null; 

        DataActor = FindObjectOfType<Actor_DataSaverLoader>();

        if (DataActor != null)
        {
            CollectSaveableEntities();
            DataActor.Act_LoadData();
        }
    }

    private void CollectSaveableEntities()
    {
        if (DataActor == null) return;

        SaveableEntity[] targets = FindObjectsByType<SaveableEntity>(FindObjectsSortMode.None);
        
        DataActor.targetObjects = new List<GameObject>();
        foreach (var target in targets)
        {
            if (target != null)
            {
                DataActor.targetObjects.Add(target.gameObject);
            }
        }
    }

    // 앱이 꺼지거나 백그라운드로 갈 때는 오브젝트가 살아있으므로 기존 로직 유지
    private void OnApplicationFocus(bool focus)
    {
        if (!focus && DataActor != null && DataActor.gameObject != null)
        {
            CollectSaveableEntities(); // 안전하게 한 번 더 수집 후 저장
            DataActor.Act_SaveData();
        }
    }

    private void OnApplicationQuit()
    {
        if (DataActor != null && DataActor.gameObject != null)
        {
            CollectSaveableEntities();
            DataActor.Act_SaveData();
        }
    }
}