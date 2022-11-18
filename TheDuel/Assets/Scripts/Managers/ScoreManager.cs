using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

public enum ScoreWindowState
{
    Before, OpenForPlayer, OpenForEnemy, Closed
}

public class ScoreManager : ManagerBase<ScoreManager>
{
    public Action<float> onPlayerScoreChanged;
    public Action<float> onEnemyScoreChanged;

    public Action<InfoAttackHit> onPlayerValidAttack;
    public Action<InfoAttackHit> onEnemyValidAttack;

    public Action<InfoAttackHit> onPlayerInvalidAttack;
    public Action<InfoAttackHit> onEnemyInvalidAttack;

    public Action onPlayerScoreWindowStarted;
    public Action onPlayerScoreWindowFailed;
    public Action onPlayerScoreWindowSucceeded;

    public Action onEnemyScoreWindowStarted;
    public Action onEnemyScoreWindowFailed;
    public Action onEnemyScoreWindowSucceeded;

    public float remainingScoreWindowTime { get; private set; } = -1f;
    public ScoreWindowState scoreWindowState { get; private set; } = ScoreWindowState.Before;
    
    public int playerScore;
    public int enemyScore;
    public Volume slowedTimeVolume;

    public float timescaleRevertSpeed = 1f;
    public float scoreWindowTime = 0.4f;
    public float scoreWindowTimescale = 0.5f;

    private void Start()
    {
        Player.instance.onDealAttack += HandlePlayerDealtAttack;
        Enemy.instance.onDealAttack += HandleEnemyDealtAttack;
        GameManager.instance.onRoundStarted = () => scoreWindowState = ScoreWindowState.Before;
        GameManager.instance.onRoundEnded = () => scoreWindowState = ScoreWindowState.Closed;
    }

    private void Update()
    {
        if (scoreWindowState == ScoreWindowState.OpenForEnemy || scoreWindowState == ScoreWindowState.OpenForPlayer)
        {
            remainingScoreWindowTime = Mathf.MoveTowards(remainingScoreWindowTime, 0, Time.unscaledDeltaTime);
            if (remainingScoreWindowTime < 0.01f)
            {
                remainingScoreWindowTime = 0f;
                if (scoreWindowState == ScoreWindowState.OpenForEnemy) onEnemyScoreWindowFailed?.Invoke();
                else onPlayerScoreWindowFailed?.Invoke();
                GameManager.instance.StartNewRound();
            }
        }
        else
        {
            Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, timescaleRevertSpeed * Time.unscaledDeltaTime);
        }
        slowedTimeVolume.weight = 1 - Time.timeScale;
    }
    
    private void HandlePlayerDealtAttack(InfoAttackHit hit)
    {
        if (scoreWindowState == ScoreWindowState.Closed)
        {
            onPlayerInvalidAttack?.Invoke(hit);
            return;
        }

        IncrementPlayerScore(hit.score);
        onPlayerValidAttack?.Invoke(hit);
        if (scoreWindowState == ScoreWindowState.Before)
        {
            StartScoreWindow(false);
        }
        else if (scoreWindowState == ScoreWindowState.OpenForPlayer)
        {
            onPlayerScoreWindowSucceeded?.Invoke();
            Time.timeScale = 0f;
            GameManager.instance.StartNewRound();

        }
    }

    private void HandleEnemyDealtAttack(InfoAttackHit hit)
    {
        if (scoreWindowState == ScoreWindowState.Closed)
        {
            onEnemyInvalidAttack?.Invoke(hit);
            return;
        }

        IncrementEnemyScore(hit.score);
        onEnemyValidAttack?.Invoke(hit);
        if (scoreWindowState == ScoreWindowState.Before)
        {
            StartScoreWindow(true);
        }
        else if (scoreWindowState == ScoreWindowState.OpenForEnemy)
        {
            onEnemyScoreWindowSucceeded?.Invoke();
            Time.timeScale = 0f;
            GameManager.instance.StartNewRound();
        }
    }

    private void StartScoreWindow(bool forPlayer)
    {
        Time.timeScale = scoreWindowTimescale;
        scoreWindowState = forPlayer ? ScoreWindowState.OpenForPlayer : ScoreWindowState.OpenForEnemy;
        remainingScoreWindowTime = scoreWindowTime;
        if (forPlayer) onPlayerScoreWindowStarted?.Invoke();
        else onEnemyScoreWindowStarted?.Invoke();
    }

    public void IncrementEnemyScore(int amount)
    {
        enemyScore += amount;
        onEnemyScoreChanged?.Invoke(enemyScore);
    }

    public void IncrementPlayerScore(int amount)
    {
        playerScore += amount;
        onPlayerScoreChanged?.Invoke(playerScore);
    }
}
