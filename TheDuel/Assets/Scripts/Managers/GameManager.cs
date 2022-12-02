using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : ManagerBase<GameManager>
{
    public Action onRoundEnded;
    public Action onRoundPrepare;
    public Action onRoundStarted;

    public bool isRoundOngoing { get; private set; }

    public Volume fadeOutVolume;

    public Transform playerStartPos;
    public Transform enemyStartPos;
    public BoxCollider playRegionCollider;
    
    public Bounds playRegion { get; private set; }
    

    public int cheatPenalty;

    private void Awake()
    {
        playRegion = playRegionCollider.bounds;
    }

    private void Start()
    {
        StartCoroutine(StartRoundRoutine());

        Referee.instance.onDetectedCheating += () =>
        {
            StartCoroutine(CheatRestartRoutine());
        };
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
        if (isRoundOngoing)
        {
            isRoundOngoing = false;
            onRoundEnded?.Invoke();   
        }
        yield return new WaitForSecondsRealtime(1f);
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 1f, 1f);
        yield return new WaitForSecondsRealtime(1f);
    }
    
    private IEnumerator StartRoundRoutine()
    {
        fadeOutVolume.weight = 1f;
        Player.instance.transform.SetPositionAndRotation(playerStartPos.position, playerStartPos.rotation);
        Enemy.instance.transform.SetPositionAndRotation(enemyStartPos.position, enemyStartPos.rotation);
        onRoundPrepare?.Invoke();
        DOTween.To(() => fadeOutVolume.weight, v => fadeOutVolume.weight = v, 0f, 1f);
        yield return new WaitForSecondsRealtime(1f);
        onRoundStarted?.Invoke();
        isRoundOngoing = true;
    }

    private IEnumerator CheatRestartRoutine()
    {
        isRoundOngoing = false;
        onRoundEnded?.Invoke();
        yield return new WaitForSeconds(1.5f);
        ScoreManager.instance.IncrementEnemyScore(cheatPenalty);
        yield return new WaitForSeconds(2f);
        StartNewRound();
    }
}
