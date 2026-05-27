using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class HW23TransformData
{
    public string objectName;
    public float posX, posY, posZ;
    public float rotX, rotY, rotZ;
}

[Serializable]
public class HW23WorldData
{
    public List<HW23TransformData> objects = new List<HW23TransformData>();
}

public class SaveLoadManager : MonoBehaviour
{
    [Header("저장할 Transform 목록")]
    public List<Transform> targetTransforms = new List<Transform>();

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "hw23_world_save.json");
        Debug.Log("Save Path: " + savePath);
    }

    void Start()
    {
        LoadWorld();
    }

    public void SaveWorld()
    {
        HW23WorldData worldData = new HW23WorldData();

        foreach (Transform target in targetTransforms)
        {
            if (target == null) continue;

            worldData.objects.Add(new HW23TransformData
            {
                objectName = target.name,
                posX = target.position.x,
                posY = target.position.y,
                posZ = target.position.z,
                rotX = target.eulerAngles.x,
                rotY = target.eulerAngles.y,
                rotZ = target.eulerAngles.z
            });
        }

        string json = JsonUtility.ToJson(worldData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("저장 완료\n" + json);
    }

    public void LoadWorld()
    {
        if (!File.Exists(savePath))
        {
            Debug.Log("저장 파일 없음");
            return;
        }

        string json = File.ReadAllText(savePath);
        HW23WorldData worldData = JsonUtility.FromJson<HW23WorldData>(json);

        foreach (HW23TransformData data in worldData.objects)
        {
            Transform target = targetTransforms.Find(t => t != null && t.name == data.objectName);
            if (target == null)
            {
                Debug.LogWarning("복원 대상 없음: " + data.objectName);
                continue;
            }

            CharacterController cc = target.GetComponent<CharacterController>();
            Rigidbody rb = target.GetComponent<Rigidbody>();

            if (cc != null)
            {
                cc.enabled = false;
                target.position = new Vector3(data.posX, data.posY, data.posZ);
                target.rotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);
                cc.enabled = true;
            }
            else if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                rb.MovePosition(new Vector3(data.posX, data.posY, data.posZ));
                rb.MoveRotation(Quaternion.Euler(data.rotX, data.rotY, data.rotZ));
            }
            else
            {
                target.position = new Vector3(data.posX, data.posY, data.posZ);
                target.rotation = Quaternion.Euler(data.rotX, data.rotY, data.rotZ);
            }
        }

        Debug.Log("불러오기 완료");
    }

    public void ResetObjects()
    {
        foreach (Transform target in targetTransforms)
        {
            if (target == null) continue;

            CharacterController cc = target.GetComponent<CharacterController>();
            Rigidbody rb = target.GetComponent<Rigidbody>();

            if (cc != null)
            {
                cc.enabled = false;
                target.position = Vector3.zero;
                target.rotation = Quaternion.identity;
                cc.enabled = true;
            }
            else if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
                target.position = Vector3.zero;
                target.rotation = Quaternion.identity;
            }
            else
            {
                target.position = Vector3.zero;
                target.rotation = Quaternion.identity;
            }
        }

        Debug.Log("위치 초기화 완료");
    }

    void OnApplicationPause(bool pause)
    {
        if (pause) SaveWorld();
    }

    void OnApplicationQuit()
    {
        SaveWorld();
    }
}