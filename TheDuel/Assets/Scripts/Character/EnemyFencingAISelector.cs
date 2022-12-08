using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFencingAISelector : MonoBehaviour
{
    public EnemyFencingAIProfile whenLosing;
    public EnemyFencingAIProfile whenSame;
    public EnemyFencingAIProfile whenWinning;
    
    private void Start()
    {
        GameManager.instance.onRoundPrepare += () =>
        {
            var ai = GetComponent<EnemyFencingAI>();
            if (ScoreManager.instance.enemyScore > ScoreManager.instance.playerScore)
            {
                ai.profile = whenWinning;
            } 
            else if (ScoreManager.instance.enemyScore < ScoreManager.instance.playerScore)
            {
                ai.profile = whenLosing;
            }
            else
            {
                ai.profile = whenSame;
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
