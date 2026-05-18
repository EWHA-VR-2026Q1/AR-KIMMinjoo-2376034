using TMPro;
using UnityEngine;

public class CanvasButtonTest : MonoBehaviour
{
    public TMP_Text overlayText;
    public TMP_Text cameraText;
    public TMP_Text worldText;

    public void ChangeOverlayText()
    {
        overlayText.text = "Overlay Button Clicked!";
    }

    public void ChangeCameraText()
    {
        cameraText.text = "Camera Button Clicked!";
    }

    public void ChangeWorldText()
    {
        worldText.text = "World Button Clicked!";
    }
}