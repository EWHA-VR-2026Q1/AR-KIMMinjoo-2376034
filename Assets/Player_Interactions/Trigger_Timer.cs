using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Timer : MonoBehaviour
{
    public GameObject InterfaceObject;
    IInteractable Interface;

    public float Interval = 1f;
    float lastTime;
    float currentTime;
    int counter;
    public bool hasCountLimit = false;
    public int CountLimit;

    private void Start()
    {
        Interface = InterfaceObject.GetComponent<IInteractable>();
    }
    void Update()
    {
        currentTime += Time.deltaTime;
        if (currentTime > lastTime + Interval)
        {
            lastTime = currentTime;
            counter++;
            Interface.OnEnter();
            Debug.Log("It's Time");
        }

        if (hasCountLimit && counter > CountLimit)
        {
            Interface.OnClick();
        }
    }
}
