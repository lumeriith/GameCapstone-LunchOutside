using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ScoreManager : ManagerBase<ScoreManager>
{
    public Action<float> onPlayerScoreChanged;
    public Action<float> onEnemyScoreChanged;
    
    public int playerScore;
    public int enemyScore;
    public Volume slowedTimeVolume;
    
    public float timescaleRevertSpeed = 1f;

    private void Start()
    {
        Player.instance.onDealAttack += HandlePlayerDealtAttack;
        Enemy.instance.onDealAttack += HandleEnemyDealtAttack;
    }

    private void Update()
    {
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, timescaleRevertSpeed * Time.unscaledDeltaTime);
        slowedTimeVolume.weight = 1 - Time.timeScale;
    }

    private void HandlePlayerDealtAttack(InfoAttackHit hit)
    {
        Time.timeScale = 0f;
        playerScore++;
        onPlayerScoreChanged?.Invoke(playerScore);
    }

    private void HandleEnemyDealtAttack(InfoAttackHit hit)
    {
        Time.timeScale = 0f;
        enemyScore++;
        onEnemyScoreChanged?.Invoke(enemyScore);
    }
}
