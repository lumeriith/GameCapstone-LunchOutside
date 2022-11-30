using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class RefereeEffectManager : ManagerBase<RefereeEffectManager>
{
    public Volume hiddenVolume;
    public Volume visibleVolume;
    public Volume detectedVolume;

    public float visibilityVolumeChangeSpeed = 2f;
    
    private void Update()
    {
        hiddenVolume.weight = Mathf.MoveTowards(hiddenVolume.weight, 1 - Referee.instance.currentVisibility,
            Time.deltaTime * visibilityVolumeChangeSpeed);
        visibleVolume.weight = Mathf.Clamp(Mathf.MoveTowards(visibleVolume.weight, Referee.instance.currentVisibility,
            Time.deltaTime * visibilityVolumeChangeSpeed), 0, 1 - detectedVolume.weight);
        detectedVolume.weight = Referee.instance.currentSuspicion;
    }
}
