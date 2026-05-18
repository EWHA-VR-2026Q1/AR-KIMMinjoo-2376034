using UnityEngine;
using UnityEngine.InputSystem; // Input System 필수 네임스페이스
using UnityEngine.Playables;
using UnityEngine.Timeline; // 타임라인 관련 클래스
using System.Linq; // 마커 검색을 위해 필요

public class EX_PointTest_JumpToTimelineMarker : MonoBehaviour
{
    public PlayableDirector director;
    public string MarkerName;

    void Awake()
    {
        if (director == null)
        {
            director = GetComponent<PlayableDirector>();
        }
    }

    void Update()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            Debug.Log("터치 입력 감지");
            Vector2 screenPosition = Pointer.current.position.ReadValue();
            ExecuteRaycast(screenPosition);
        }
    }

    void ExecuteRaycast(Vector2 pos)
    {
        Ray ray = Camera.main.ScreenPointToRay(pos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Debug.Log($"터치 발생. {MarkerName} 마커로 점프");
            Act_JumpToMarker(MarkerName);
        }
    }

    public void Act_JumpToMarker(string markerName)
    {
        TimelineAsset timeline = director.playableAsset as TimelineAsset;

        if (timeline == null)
        {
            Debug.LogError("타임라인 에셋을 찾을 수 없습니다.");
            return;
        }

        // GetOutputTracks()를 호출
        var markers = timeline.GetOutputTracks()
            .SelectMany(t => t.GetMarkers())
            .OfType<SignalEmitter>();

        foreach (var m in markers)
        {
            // 이미터 에셋 이름이 아닌, 타임라인 창에서 설정한 '이름'으로 비교하려면
            // 시그널 에셋 자체의 비교 혹은 특별한 네이밍 규칙이 필요
            if (m.asset != null && m.asset.name == markerName)
            {
                director.time = m.time;
                director.Play();
                return;
            }
        }
    }

    // 타임라인 일시정지
    public void Act_PauseTimelineForPlay()
    {
        director.Pause();
        HideMouse();
        Debug.Log("Pause for Play");
    }

    public void Act_PauseTimelineForUI()
    {
        director.Pause();
        ShowMouse();
        Debug.Log("Pause for UI");
    }

    void ShowMouse()
    {
        //Debug.Log("showMouse");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void HideMouse()
    {
        //Debug.Log("hideMouse");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}