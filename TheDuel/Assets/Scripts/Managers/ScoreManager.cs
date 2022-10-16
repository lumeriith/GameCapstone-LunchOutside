using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : ManagerBase<ScoreManager>
{
    public Action onPlayerScoreChanged;
    public Action onEnemyScoreChanged;
    
    public int playerScore;
    public int enemyScore;
    
    public float timescaleRevertSpeed = 1f;

    private void Start()
    {
        Player.instance.onDealAttack += HandlePlayerDealtAttack;
        Enemy.instance.onDealAttack += HandleEnemyDealtAttack;
    }

    private void Update()
    {
        Time.timeScale = Mathf.MoveTowards(Time.timeScale, 1, timescaleRevertSpeed * Time.unscaledDeltaTime);
    }

    private void HandlePlayerDealtAttack(InfoAttackHit hit)
    {
        Time.timeScale = 0f;
        playerScore++;
        onPlayerScoreChanged?.Invoke();
    }

    private void HandleEnemyDealtAttack(InfoAttackHit hit)
    {
        Time.timeScale = 0f;
        enemyScore++;
        onEnemyScoreChanged?.Invoke();
    }
}
