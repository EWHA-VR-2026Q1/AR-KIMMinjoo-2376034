using UnityEngine;

public class Actor_Print : MonoBehaviour
{
    [Header("Console 창에 메시지를 출력하는 클래스")]
    [Header("--------")]
    [Header("Act_PrintMessage(string): \nConsole 창에 string을 출력함")]
    [Header("Act_PrintMessage(): \nConsole 창에 Message의 텍스트를 출력함")]
    [Header("--------")]

    [Tooltip("출력할 메시지 입력")]
    public string Message;

    /// <summary>
    /// 메시지를 출력하는 함수
    /// message 매개변수로 전달된 문자열을 출력
    /// </summary>
    /// <param name="message">출력할 메시지</param>
    public void Act_PrintMessage(string message)
    {
        Debug.Log("<color=magenta>Actor_Print: " + message + "</color>");
    }

    /// <summary>
    /// 인스펙터에서 메시지를 출력하는 함수
    /// 컴포넌트의 Message 필드에 입력한 메시지 출력
    /// </summary>
    public void Act_PrintMessage()
    {
        Debug.Log("<color=magenta>Actor_Print: " + Message + "</color>");
    }
}
