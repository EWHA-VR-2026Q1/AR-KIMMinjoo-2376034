using UnityEngine;

public class Actor_Audio : MonoBehaviour
{
    [Header("오디오를 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_Play(): \n재생")]
    [Header("Act_Stop(): \n정지")]
    [Header("Act_Pause(): \n일시정지")]
    [Header("Act_PlayStopToggle(): \n재생-정지 토글")]
    [Header("Act_PlayPauseToggle(): \n재생-일시정지 토글")]
    [Header("--------")]

    [Tooltip("AudioSource를 갖고 있는 게임오브젝트 할당. \n미할당시 현재 게임오브젝트의 AudioSource를 찾음")]
    public AudioSource Player;
    [Tooltip("시작할 때 Audio를 재생할지의 여부. 체크시 재생.")]
    public bool isPlayOnStart = false;

    float defaultVolume;

    void Awake()
    {
        if (Player != null)
        {
            defaultVolume = Player.volume;
            Debug.Log("AudioSource detected.");
        }
        else
        {
            Player = GetComponent<AudioSource>();
            if (Player != null)
            {
                defaultVolume = Player.volume;
            }
            else
            {
                Debug.LogWarning("AudioSource 없음!");
            }
        }
        if (isPlayOnStart)
        {
            Act_Play();
        }
        else
        {
            Act_Stop();
        }
    }

    /// <summary>
    /// 재생
    /// </summary>
    public void Act_Play()
    {

        Player.volume = defaultVolume;
        Player.Play();

    }

    /// <summary>
    /// 정지
    /// </summary>
    public void Act_Stop()
    {
        Player.Stop();
    }

    /// <summary>
    /// 일시정지
    /// </summary>
    public void Act_Pause()
    {
        Player.Pause();

    }

    /// <summary>
    /// 재생-정지 토글
    /// </summary>
    public void Act_PlayStopToggle()
    {
        if (Player.isPlaying)
        {
            Act_Stop();
        }
        else
        {
            Act_Play();
        }        
    }

    /// <summary>
    /// 재생-일시정지 토글
    /// </summary>
    public void Act_PlayPauseToggle()
    {
        if (Player.isPlaying)
        {
            Act_Pause();
        }
        else
        {
            Act_Play();
        }
    }
}