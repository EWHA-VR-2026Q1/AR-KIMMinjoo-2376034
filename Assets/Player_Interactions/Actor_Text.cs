using UnityEngine;
using TMPro;

public class Actor_Text : MonoBehaviour
{
    [Header("TMP_Text 컴포넌트에 텍스트를 출력하는 클래스")]
    [Header("--------")]
    [Header("Act_SetText(string): \nstring을 표시함")]
    [Header("Act_SetText(): \nActor_Text의 text 변수에 입력한 텍스트를 표시함")]
    [Header("--------")]

    [Tooltip("텍스트를 표시할 TMP_Text 컴포넌트")]
    public TMP_Text TMPText;
    [Tooltip("TMP_Text 컴포넌트에 표시할 텍스트")]
    public string text;
    
    public void Act_SetText(string text)
    {
        this.text = text;
        TMPText.text = this.text;
    }

    public void Act_SetText()
    {
        TMPText.text = text;
    }
}
