using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class Actor_Video : MonoBehaviour
{
    [Header("Video를 제어하는 클래스")]
    [Header("--------")]
    [Header("Act_Play(): 재생")]
    [Header("Act_Stop(): 정지")]
    [Header("Act_Pause(): 일시정지")]
    [Header("Act_PlayStopToggle(): 재생-정지 토글")]
    [Header("Act_PlayPauseToggle(): 재생-일시정지 토글")]
    [Header("--------")]

    [Tooltip("VideoPlayer를 가진 게임오브젝트 할당. 미할당시 현재 게임오브젝트에서 Renderer를 찾음.")]
    public VideoPlayer Player;
    [Tooltip("시작할 때 Video를 재생할지의 여부. 체크시 재생.")]
    public bool isPlayOnStart = false;
    Renderer Renderer;
    Color color;
    bool hasMat = false;

    void Awake()
    {
        Renderer = Player.gameObject.GetComponent<Renderer>();
        if (Renderer != null)
        {
            hasMat = true;
            color = Renderer.material.GetColor("_Color");
            Debug.Log($"{gameObject.name} has renderer.");
        }
        else
        {
            Debug.Log($"{gameObject.name} has NO senderer.");
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

    public void Act_Play()
    {
        if (hasMat)
        {
            Renderer.material.SetColor("_Color", Color.white);
        }
        Player.Play();
    }

    public void Act_Stop()
    {
        if (hasMat)
        {
            Renderer.material.SetColor("_Color", color);
        }
        Player.Stop();
    }

    public void Act_Pause()
    {
        Player.Pause();
    }

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
