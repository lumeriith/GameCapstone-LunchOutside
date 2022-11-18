using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class RefereeCutsceneCamera : MonoBehaviour
{
    private CinemachineVirtualCamera _vcam;

    private void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
    }

    private void Start()
    {
        Referee.instance.onDetectedCheating += () =>
        {
            _vcam.enabled = true;
        };

        GameManager.instance.onRoundPrepare += () =>
        {
            _vcam.enabled = false;
        };
    }
}
