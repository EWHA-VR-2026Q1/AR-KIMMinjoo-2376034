using UnityEngine;


public interface IInteractable
{
    // 상태 변화 (Hover)
    // 인터랙션 대상 근처에 가거나 벗어날 때
    void OnEnter();
    void OnExit();
    void OnStay();
   // 실행 동작 (Click)
    // 클릭하거나, 손가락 끝으로 찌르거나(Poke), 엄지와 검지로 집을 때(Pinch)
    // 주로 '버튼 누르기'나 '선택'의 의미
    void OnClick();

    // 호출자(GameObject)를 매개변수로 받도록 설계 추가
    void OnEnter(GameObject sender);
    void OnExit(GameObject sender);
    void OnStay(GameObject sender);
    void OnClick(GameObject sender);
}
