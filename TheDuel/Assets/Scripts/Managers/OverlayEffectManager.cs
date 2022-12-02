using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class OverlayEffectManager : ManagerBase<OverlayEffectManager>
{
    public Volume hiddenVolume;
    public Volume visibleVolume;
    public Volume detectedVolume;
    public float visibilityVolumeChangeSpeed = 2f;

    public Volume tiredVolume;
    public float tiredStaminaStart = 0.5f;
    public float tiredVolumeSpeed = 2f;
    
    private void Update()
    {
        hiddenVolume.weight = Mathf.MoveTowards(hiddenVolume.weight, 1 - Referee.instance.currentVisibility,
            Time.unscaledDeltaTime * visibilityVolumeChangeSpeed);
        visibleVolume.weight = Mathf.Clamp(Mathf.MoveTowards(visibleVolume.weight, Referee.instance.currentVisibility,
            Time.unscaledDeltaTime * visibilityVolumeChangeSpeed), 0, 1 - detectedVolume.weight);
        detectedVolume.weight = Referee.instance.currentSuspicion;

        var targetWeight = Player.instance.stamina < tiredStaminaStart ? (tiredStaminaStart - Player.instance.stamina) / tiredStaminaStart : 0;
        tiredVolume.weight = Mathf.MoveTowards(tiredVolume.weight, targetWeight, Time.unscaledDeltaTime * tiredVolumeSpeed);
    }
}
