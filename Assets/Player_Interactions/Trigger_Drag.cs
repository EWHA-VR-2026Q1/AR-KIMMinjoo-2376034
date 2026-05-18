using UnityEngine;
using UnityEngine.InputSystem;

public class Trigger_Drag : MonoBehaviour
{
    private Camera mainCamera;
    private bool isDragging = false;
    private float zDistance;
    private Vector3 offset; // 클릭 지점과 오브젝트 중심 사이의 차이 저장

    [Header("Target Interface")]
    public GameObject InterfaceObject; // 인터페이스가 붙어있는 게임 오브젝트를 지정할 수 있도록 public으로 선언
    private IInteractable Interface;

    private void Awake()
    {
        mainCamera = Camera.main;
        Interface = GetComponent<IInteractable>();
    }

    void Update()
    {
        // 포인터(마우스/터치) 장치 확인
        var pointer = Pointer.current;
        if (pointer == null) return;

        // 누르기 시작했을 때 (OnMouseDown 대체)
        if (pointer.press.wasPressedThisFrame)
        {
            StartDrag(pointer.position.ReadValue());
            Interface.OnEnter(gameObject);
        }

        // 떼었을 때 (OnMouseUp 대체)
        if (pointer.press.wasReleasedThisFrame)
        {
            isDragging = false;
            Interface.OnExit(gameObject);
        }

        // 드래그 중일 때 (OnMouseDrag 대체)
        if (isDragging)
        {
            ExecuteDrag(pointer.position.ReadValue());
            Interface.OnStay(gameObject);
        }
    }

    private void StartDrag(Vector2 screenPos)
    {
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform == transform) // 자기 자신을 클릭했는지 확인
            {
                isDragging = true;
                // 현재 오브젝트와 카메라 사이의 깊이(Z) 값을 저장
                zDistance = mainCamera.WorldToScreenPoint(gameObject.transform.position).z;
                
                // Offset 계산. 클릭한 월드 좌표 구하기
                Vector3 clickWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, zDistance));

                // 오브젝트 중심점과 클릭 지점의 차이(Offset) 저장
                if (transform.parent != null)
                {
                    offset = transform.localPosition - transform.parent.InverseTransformPoint(clickWorldPos);
                }
                else
                {
                    offset = transform.position - clickWorldPos;
                }
            }
        }
    }

    private void ExecuteDrag(Vector2 screenPos)
    {
        // 스크린 좌표를 월드 좌표로 변환
        Vector3 mousePoint = new Vector3(screenPos.x, screenPos.y, zDistance);
        Vector3 newPosition = mainCamera.ScreenToWorldPoint(mousePoint);

        // 기존 코드처럼 Y축은 고정하고 X, Z축만 이동
        transform.position = new Vector3(newPosition.x, transform.position.y, newPosition.z);
    }
}