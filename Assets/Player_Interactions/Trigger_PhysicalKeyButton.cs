using UnityEngine;
using UnityEngine.InputSystem;

public class Trigger_PhysicalKeyButton : MonoBehaviour
{
    [Header("글로벌 키보드 입력 컨트롤러")]
    [Header("키 상태(Down/Stay/Up)를 Interface 메서드와 매핑함")]
    [Header("--------")]

    [Header("Input Settings")]
    public InputActionProperty Key;

    [Header("Target Interface")]
    public GameObject InterfaceObject;
    private IInteractable InterfaceBase;

    [Header("Sender Settings")]
    [Tooltip("이벤트를 보낼 때 매개변수로 전달할 게임 오브젝트. 비어있으면 자기 자신을 전달")]
    public GameObject SenderObject;

    void Awake()
    {
        // InterfaceObject가 비어있을 경우에 대비한 방어 코드
        if (InterfaceObject == null) InterfaceObject = gameObject;
        
        InterfaceBase = InterfaceObject.GetComponent<IInteractable>();
        if (SenderObject == null) SenderObject = gameObject;
    }

    void Update()
    {
        if (InterfaceBase == null) return;

        // 1. 키를 처음 눌렀을 때 -> OnEnter와 매핑
        if (Key.action.WasPressedThisFrame()) 
        {
            // Debug.Log($"Key Pressed (Enter): {Key.action.name}");
            InterfaceBase.OnEnter(SenderObject);
        }

        // 2. 키를 누르고 있는 동안 -> OnAction과 매핑 (매 프레임 호출)
        if (Key.action.IsPressed()) 
        {
            InterfaceBase.OnStay(SenderObject);
        }

        // 3. 키를 뗐을 때 -> OnExit와 매핑
        if (Key.action.WasReleasedThisFrame()) 
        {
            // Debug.Log($"Key Released (Exit): {Key.action.name}");
            InterfaceBase.OnClick(SenderObject);
            InterfaceBase.OnExit(SenderObject);
        }
    }

    private void OnEnable() => Key.action?.Enable();
    private void OnDisable() => Key.action?.Disable();
}