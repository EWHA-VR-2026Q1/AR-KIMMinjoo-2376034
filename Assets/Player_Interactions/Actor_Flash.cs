using UnityEngine;

public class Actor_Flash : InterfaceBase_IItem
{
    [Header("Flashlight Settings")]
    public Light spotLight;

    private void Start()
    {
        OnStopUse();
    }
    public override void OnUse()
    {

        spotLight.enabled = true;
        Debug.Log($"플래시 켜짐 (On)");
        // Debug.Log($"플래시 {(spotLight.enabled ? "켜짐" : "꺼짐")}");
    }

    public override void OnStopUse()
    {
        Debug.Log($"플래시 꺼짐 (Off)");
        spotLight.enabled = false;
        // Debug.Log($"플래시 {(spotLight.enabled ? "켜짐" : "꺼짐")}");
    }
}