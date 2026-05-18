using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_LifeCycle : MonoBehaviour
{
    [Header("게임오브젝트의 Life Cycle을 감지하여 에 따라 Interface(Event)로 전달하는 클래스.")]
    [Header("--------")]
    [Header("게임오브젝트 활성화 --> Interface의 OnEnterEvent 유발")]
    [Header("게임오브젝트 지속 --> Interface의 OnActionEvent 유발. 사용 자제할 것")]
    [Header("게임오브젝트 비성화 --> Interface의 OnExitEvent 유발")]
    [Header("--------")]

    public GameObject InterfaceObject; // 인터페이스가 붙어있는 게임 오브젝트를 지정할 수 있도록 public으로 선언
    IInteractable Interface;
    //public _WIP_Actor_Text TextActor;

    [Header("지연된 실행: 카운더")]
    public float maxCount = 10f;
    //bool isCount;
    //float currentCount = 0;

    [Header("지연된 실행: 타이머")]
    public float maxTime = 10f;
    bool isRun;

    void Awake()
    {
        if (InterfaceObject == null)
        {
            Interface = GetComponent<IInteractable>();
        }
        else
        {
            Interface = InterfaceObject.GetComponent<IInteractable>();
        }
    }

    void Update()
    {
        Interface.OnStay();
    }

    /*
    private void Start()
    {
        Interface.OnAction();
    }
    private void Update()
    {
        if (isCount)
        {
            Count();
            //OutputText();
        }

        if (isRun)
        {
            Timer();
            //OutputText();
        }
    }
    */

    private void OnEnable()
    {
        Debug.Log($"{gameObject.name} enabled.");
        Interface.OnEnter();
    }

    private void OnDisable()
    {
        Debug.Log($"{gameObject.name} disabled.");
        Interface.OnExit();
    }

    /*
    public void StartCounter()
    {
        isCount = true;
    }

    public void StartCounter(int _maxCount)
    {
        maxCount = _maxCount;
        isCount = true;
    }

    void Count()
    {
        currentCount++;
        if (currentCount >= maxCount)
        {
            currentCount = 0;
            isCount = false;
            Interface.OnAction();
        }
    }

    public void StartTimer(float _maxTime)
    {
        maxTime = _maxTime;
        isRun = true;
    }

    public void StartTimer()
    {
        isRun = true;
    }

    void Timer()
    {
        currentCount += Time.deltaTime;
        if (currentCount >= maxTime)
        {
            currentCount = 0;
            isRun = false;
            Interface.OnAction();
        }
    }

    public void OutputText(Actor_Text TextActor)
    {
        if (TextActor == null) return;
        TextActor.Act_SetText();
    }
    */
}
