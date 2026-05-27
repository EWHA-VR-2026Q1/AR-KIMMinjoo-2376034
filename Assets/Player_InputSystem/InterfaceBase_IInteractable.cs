using UnityEngine;
using UnityEngine.Events;

// 인스펙터에 확실히 보이게 하기 위한 선언
[System.Serializable]
public class InteractableEvent : UnityEvent<GameObject> { }

public class InterfaceBase_IInteractable : MonoBehaviour, IInteractable
{
    [Header("Interface(Event)와 Actor를 연결하는 클래스. \n+를 클릭하여 Actor들을 할당함.")]
    [Header("Animator가 있는 게임오브젝트에는 부착하지 말 것!")]
    [Header("--------")]
    [Header("OnEnterEvent(GameObject): \n상호작용 대상이 근처에 왔을 때 작동함.")]
    [Header("OnActionEvent(GameObject): \n상호작용 대상을 클릭하거나 쥐었을 때 작동함.")]
    [Header("OnExitEvent(GameObject): \n상호작용 대상이 나갔을 때 작동함.")]
    [Header("--------")]
    
    [Header("Information Settings")]
    [Tooltip("Enter Event에서 작동시킬 1개 이상의 Actor를 연결할 수 있음")]
    public InteractableEvent OnEnterEvent;

    [Tooltip("Action Event에서 작동시킬 1개 이상의 Actor를 연결할 수 있음")]
    public InteractableEvent OnStayEvent;

    [Tooltip("Exit Event에서 작동시킬 1개 이상의 Actor를 연결할 수 있음")]
    public InteractableEvent OnExitEvent;

    [Tooltip("Click Event에서 작동시킬 1개 이상의 Actor를 연결할 수 있음")]
    public InteractableEvent OnClickEvent;

    // 인터페이스 구현    
    // 매개변수 없는 버전들
    public virtual void OnEnter() => OnEnter(gameObject);
    public virtual void OnStay() => OnStay(gameObject);
    public virtual void OnExit() => OnExit(gameObject);
    public virtual void OnClick() => OnClick(gameObject);

    // [중요] 매개변수 있는 버전들에 virtual을 반드시 붙여야 자식에서 override 가능
    public virtual void OnEnter(GameObject sender) => OnEnterEvent?.Invoke(sender);
    public virtual void OnStay(GameObject sender) => OnStayEvent?.Invoke(sender);
    public virtual void OnExit(GameObject sender) => OnExitEvent?.Invoke(sender);
    public virtual void OnClick(GameObject sender) => OnClickEvent?.Invoke(sender);

    public void OnInteract(GameObject sender)
    {
        // 기본적으로 OnClick과 동일하게 작동하도록 구현
        // OnClick(sender);
        Debug.Log($"OnInteract called by {sender.name} on {gameObject.name}");
    }
}



