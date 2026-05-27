using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_LookAt : MonoBehaviour
{
    public Transform Subject;
    public Transform Target;
    public float heightOffset = 0.5f; // 부모 머리 위로 얼마나 띄울 것인가

    Transform parent;


    void Start()
    {
        if (Subject == null) Subject = transform;
        if(Target == null) Target = Camera.main.transform;
        if (Subject.parent != null) parent = Subject.parent;
    }

    void Update()
    {
        if (PlayerManager.Instance.CurrentObject == gameObject)
        {
            Subject.gameObject.SetActive(false);
        }
    }

    public void Act_LookAt()
    {
        if (parent == null) return;
        // if (PlayerManager.Instance.CurrentObject == gameObject)
        // {
        //     Subject.gameObject.SetActive(false);
        // }
        // else
        // {
        //     Subject.gameObject.SetActive(true);
        // }
        Subject.position = parent.position + (Vector3.up * heightOffset);
        // Subject.LookAt(Target.position + Target.forward, Target.up);
        Subject.LookAt(Target);
    }

    public void Act_LookAt(Transform TargetTransform)
    {
        Target = TargetTransform;
        Act_LookAt();
    }
}
