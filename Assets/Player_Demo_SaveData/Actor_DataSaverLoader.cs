using UnityEngine;
using System.Collections.Generic;
using System.IO;
using UnityEngine.SceneManagement; // 씬 이름을 가져오기 위해 추가

// ... (TransformData, WorldData 정의는 기존과 동일)

public class Actor_DataSaverLoader : MonoBehaviour
{
    [Header("Objects To Save")]
    public List<GameObject> targetObjects = new List<GameObject>();

    // [수정] 고정된 savePath 대신 현재 씬 이름을 기반으로 경로를 반환하는 프로퍼티를 씁니다.
    private string GetSavePath()
    {
        // 예: 현재 씬 이름이 "Stage_01" 이라면 "Stage_01_worldData.json"으로 저장됨
        string currentSceneName = SceneManager.GetActiveScene().name;
        return Path.Combine(Application.persistentDataPath, currentSceneName + "_worldData.json");
    }

    private void Awake()
    {
        // [수정] 기존 Awake나 Start에 있던 savePath = ... 지우기 (지워도 무방)
    }

    public void Act_SaveData()
    {
        if (targetObjects.Count == 0) return; // 저장할 대상이 없으면 스킵

        WorldData data = new WorldData();

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            TransformData t = new TransformData();
            t.objectName = obj.name;
            t.position = obj.transform.position;
            t.rotation = obj.transform.rotation;

            data.objects.Add(t);
        }

        string json = JsonUtility.ToJson(data, true);
        
        // [수정] 실시간 씬 이름 경로로 저장
        string path = GetSavePath();
        File.WriteAllText(path, json);

        Debug.Log($"[{SceneManager.GetActiveScene().name}] 저장 완료! 경로: {path}");
    }

    public void Act_LoadData()
    {
        // [수정] 실시간 씬 이름 경로에서 가져옴
        string path = GetSavePath();

        if (!File.Exists(path))
        {
            // ★ 매우 중요: 이 씬을 처음 방문했거나 파일이 없다면 기존 배치 상태 그대로 놔둡니다.
            Debug.Log($"[{SceneManager.GetActiveScene().name}] 처음 방문한 씬이거나 세이브 파일이 없어 기본 위치를 유지합니다.");
            return;
        }

        string json = File.ReadAllText(path);
        WorldData data = JsonUtility.FromJson<WorldData>(json);

        if (data == null || data.objects == null) return;

        Dictionary<string, TransformData> savedDataMap = new Dictionary<string, TransformData>();
        foreach (var savedObject in data.objects)
        {
            if (!savedDataMap.ContainsKey(savedObject.objectName))
            {
                savedDataMap.Add(savedObject.objectName, savedObject);
            }
        }

        foreach (GameObject obj in targetObjects)
        {
            if (obj == null) continue;

            if (savedDataMap.TryGetValue(obj.name, out TransformData targetData))
            {
                obj.transform.position = targetData.position;
                obj.transform.rotation = targetData.rotation;
                
                if (obj.TryGetComponent<Rigidbody>(out Rigidbody rb))
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        Debug.Log($"[{SceneManager.GetActiveScene().name}] 로드 완료!");
    }

    // 초기화 함수도 현재 씬 파일만 지우도록 수정
    public void Act_ResetAndClearData()
    {
        string path = GetSavePath();
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"{SceneManager.GetActiveScene().name} 씬의 데이터가 초기화되었습니다.");
        }
    }
}