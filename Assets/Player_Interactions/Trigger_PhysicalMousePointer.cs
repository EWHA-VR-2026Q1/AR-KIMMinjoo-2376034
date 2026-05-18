using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEditor.EditorTools;

public class Trigger_PhysicalMousePointer : MonoBehaviour
{
    
    private Camera _mainCamera;

    [Header("Target Interface")]
    public GameObject InterfaceObject; // 인터페이스가 붙어있는 게임 오브젝트를 지정할 수 있도록 public으로 선언
    private IInteractable InterfaceBase;

    [Header("Sender Settings")]
    [Tooltip("이벤트를 보낼 때 매개변수로 전달할 게임 오브젝트. 비어있으면 자기 자신을 전달")]
    public GameObject SenderObject;
    private void Awake()
    {
        _mainCamera = Camera.main;
        if(InterfaceObject == null) InterfaceObject = gameObject;
                InterfaceBase = InterfaceObject.GetComponent<IInteractable>();
        if (SenderObject == null) SenderObject = gameObject;
    }

    void Update()
    {
        // 클릭/터치 입력 감지 (New Input System 방식)
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            IInteractable interactableInterface = InterfaceObject.GetComponent<IInteractable>();
            Vector2 screenPosition = Pointer.current.position.ReadValue();

            // UI 클릭 여부 확인
            // IsPointerOverGameObject는 현재 포인터 아래에 UI(EventSystem 대상)가 있는지 체크합니다.
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                // UI를 클릭한 경우이므로 게임 로직은 실행하지 않고 종료
                Debug.Log("UI 클릭됨");
                //generalInterface?.OnClick(SenderObject); // UI 클릭도 OnClick 이벤트로 처리할 수 있도록 호출
                return;
            }

            // 게임 오브젝트 클릭 처리 (Raycast)
            // HandleGameObjectClick(screenPosition);
            Ray ray = _mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // 여기서 클릭된 오브젝트에 따른 로직 수행
                Debug.Log($"오브젝트 클릭됨: {hit.transform.name}");
                
                // 인터페이스나 특정 컴포넌트 호출
                interactableInterface?.OnClick(SenderObject);
            }
        }
    }
}