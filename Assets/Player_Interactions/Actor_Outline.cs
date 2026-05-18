using UnityEngine;

[RequireComponent(typeof(Outline))]
public class Actor_Outline : MonoBehaviour
{
    [Header("Outline을 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_Outline_Show(): Outline을 표시함.")]
    [Header("Act_Outline_Hide(): Outline을 숨김.")]
    [Header("--------")]

    [Tooltip("가이드용 더미 변수. 체크 무관.")]
    [SerializeField]
    private bool _inspectorGuide; // 가이드용 더미 변수

    Color OutlineColor;
    private Outline outline;

    void Awake()
    {
        outline = GetComponent<Outline>();
        outline.enabled = false; // 시작할 때는 끔
        OutlineColor = outline.OutlineColor;
    }

    public void Act_Outline_Show()
    {
        outline.enabled = true;
        outline.OutlineColor = OutlineColor;
    }


    public void Act_Outline_Hide()
    {
        outline.enabled = false;
        outline.OutlineColor = OutlineColor;
    }

    public void Act_Outline_Red()
    {
        Debug.Log($"{gameObject.name}을 클릭했습니다!");
        outline.OutlineColor = Color.red; // 클릭 시 색상 변경 예시
    }
}