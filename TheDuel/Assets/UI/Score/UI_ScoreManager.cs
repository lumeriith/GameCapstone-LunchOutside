using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI playerScore;
    public TextMeshProUGUI enemyScore;
    public float dotDisplayTime = 2f;

    public GameObject playerScoreDot;
    public GameObject enemyScoreDot;
    
    private float _lastPlayerScoreTime = float.NegativeInfinity;
    private float _lastEnemyScoreTime = float.NegativeInfinity;

    private void Start()
    {
        ScoreManager.instance.onPlayerScoreChanged += UpdatePlayerScore;
        ScoreManager.instance.onEnemyScoreChanged += UpdateEnemyScore;
        playerScoreDot.SetActive(false);
        enemyScoreDot.SetActive(false);
    }

    private void Update()
    {
        playerScoreDot.SetActive(Time.unscaledTime - _lastPlayerScoreTime < dotDisplayTime);
        enemyScoreDot.SetActive(Time.unscaledTime - _lastEnemyScoreTime < dotDisplayTime);
    }

    private void UpdatePlayerScore(float newScore)
    {
        playerScore.text = newScore.ToString();
        _lastPlayerScoreTime = Time.unscaledTime;
    }

    private void UpdateEnemyScore(float newScore)
    {
        enemyScore.text = newScore.ToString();
        _lastEnemyScoreTime = Time.unscaledTime;
    }
}
