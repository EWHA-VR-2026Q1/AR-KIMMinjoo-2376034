using UnityEngine;
using UnityEngine.UI;

public class UIInteractableAdapter : MonoBehaviour, IInteractable
{
    private Button targetButton;

    private void Awake()
    {
        targetButton = GetComponent<Button>();
    }

    // 인터랙션 대상 근처에 가거나 벗어날 때
    public void OnEnter() {  }
    public void OnExit() { }
    public void OnStay() { }
    // 실행 동작 (Click)
    // 클릭하거나, 손가락 끝으로 찌르거나(Poke), 엄지와 검지로 집을 때(Pinch)
    // 주로 '버튼 누르기'나 '선택'의 의미
    public void OnClick() { }

    public void OnClick(GameObject pointer)
    {
        // UI 버튼의 onClick 이벤트에 등록된 함수들을 강제로 실행
        if (targetButton != null && targetButton.interactable)
        {
            targetButton.onClick.Invoke();
        }
    }

    public void OnEnter(GameObject pointer) { /* 필요 시 버튼 하이라이트 연출 */ }
    public void OnStay(GameObject pointer) { }
    public void OnExit(GameObject pointer) { }
}