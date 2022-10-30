using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : ManagerBase<GameManager>
{
    public Action onRoundEnded;
    public Action onRoundStarted;

    public bool isRoundOngoing { get; private set; }

    public Volume fadeOutVolume;

    public Transform playerStartPos;
    public Transform enemyStartPos;

    private void Start()
    {
        StartCoroutine(StartRoundRoutine());
    }

    public void StartNewRound()
    {
        StartCoroutine(StartNewRoundRoutine());
    }

    private IEnumerator StartNewRoundRoutine()
    {
        yield return EndRoundRoutine();
        yield return StartRoundRoutine();
    }

    private IEnumerator EndRoundRoutine()
    {
        isRoundOngoing = false;
        onRoundEnded?.Invoke();
        yield return new WaitForSecondsRealtime(1f);
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
    }
    
    private IEnumerator StartRoundRoutine()
    {
        fadeOutVolume.weight = 1f;
        Player.instance.transform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        Enemy.instance.transform.SetPositionAndRotation(enemyStartPos.position, enemyStartPos.rotation);
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 0f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        onRoundStarted?.Invoke();
        isRoundOngoing = true;
    }
}
