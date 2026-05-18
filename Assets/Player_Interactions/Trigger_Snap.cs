using UnityEngine;
using UnityEngine.InputSystem;

public class Trigger_Snap : MonoBehaviour
{
    [Header("스냅 설정. SnapPoint 태그를 가진 오브젝트와 스냅")]
    [SerializeField] private string snapPointTag = "Target";

    private bool hasHitSnapPoint = false;
    private Vector3 snapTargetPosition;

    void Update()
    {
        // New Input System에서 마우스 왼쪽 버튼 또는 터치 뗌 감지
        if (Pointer.current != null && Pointer.current.press.wasReleasedThisFrame)
        {
            CheckAndSnap();
            GetComponent<IInteractable>().OnClick();
        }
    }

    private void CheckAndSnap()
    {
        // 마우스를 놓았을 때 스냅 포인트 영역 안이라면 위치 고정
        if (hasHitSnapPoint)
        {
            transform.position = snapTargetPosition;
            hasHitSnapPoint = false; // 스냅 완료 후 상태 초기화 (필요에 따라 유지 가능)
            Debug.Log($"[Snap] {gameObject.name}이(가) {snapTargetPosition}으로 스냅되었습니다.");
        }
    }

    // 트리거 감지 로직
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(snapPointTag))
        {
            hasHitSnapPoint = true;
            snapTargetPosition = other.transform.position;
            Debug.Log("Snap Area Enter");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(snapPointTag))
        {
            hasHitSnapPoint = false;
            Debug.Log("Snap Area Exit");
        }
    }
}