using System.Linq; // 마커 검색을 위해 필요
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline; // 타임라인 관련 클래스

public class Actor_Timeline : MonoBehaviour
{
    [Header("Timeline을 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_PlayTimeline(): \n타임라인 재생")]
    [Header("Act_PauseTimelineForPlay(): \n게임 플레이를 위한 타임라인 일시정지. 마우스 숨김.")]
    [Header("Act_PauseTimelineForUI(): \nUI를 위한 타임라인 일시정지. 마우스 나타남.")]
    [Header("Act_ResumeTimeline(): \n타임라인 이어서 재생.")]
    [Header("Act_StopTimeline(): \n타임라인 정지.")]
    [Header("Act_JumpToMarker(): \n특정한 마커로 점프.")]
    [Header("Act_QuitGame(): \n유니티 에디터 종료.")]
    [Header("--------")]

    [Tooltip("PlayableDirector 컴포넌트 할당. 미할당시 현재 게임오브젝트의 PlayableDirector를 찾음.")]
    public PlayableDirector director;

    //string nextMarker = "";

    void Awake()
    {
        if(director == null)
        {
            director = GetComponent<PlayableDirector>();
        }
    }
    public void Act_PlayTimeline()
    {
        director.Play();
        Debug.Log("Play");
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

    public void Act_ResumeTimeline()
    {
        director.Resume();
        Debug.Log("Resume");
    }

    // 타임라인 완전 정지
    public void Act_StopTimeline()
    {
        director.Stop();
        Debug.Log("Stop");
    }

    public void Act_JumpToMarker(string markerName)
    {
        // playableAsset을 TimelineAsset으로 형변환
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

    public void Act_QuitGame()
    {
#if UNITY_EDITOR
        // 유니티 에디터에서 실행 중일 때
        UnityEditor.EditorApplication.isPlaying = false;
#else
                // 실제 빌드된 앱에서 실행 중일 때
                Application.Quit();
#endif
    }

    /*
    public void Act_JumpToFrame(int frameIndex)
    {
        double targetTime = (double)frameIndex / 60.0; // 60fps 기준
        director.time = targetTime;

        // Evaluate()를 주석 처리하거나 삭제하고 Play()를 호출해 보세요.
        director.Play();
    }

    public void Act_JumpToClipStart(string trackName, string clipName)
    {
        // 1. 타임라인 에셋에서 특정 트랙을 찾습니다.
        TimelineAsset timeline = director.playableAsset as TimelineAsset;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track.name == trackName)
            {
                // 2. 해당 트랙 안에 있는 클립들 중 이름을 확인합니다.
                foreach (var clip in track.GetClips())
                {
                    if (clip.displayName == clipName)
                    {
                        director.time = clip.start; // 클립의 시작 시간으로 점프!
                        director.Play();
                        return;
                    }
                }
            }
        }
    }

    public void Act_SetNextMarker(string markerName)
    {
        nextMarker = markerName;
        Debug.Log("nextMarker: " + nextMarker);
    }
    */

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
