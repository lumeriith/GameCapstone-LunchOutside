using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HitMarkerManager : MonoBehaviour
{
    public UI_HitMarker playerHitMarker;
    public UI_HitMarker enemyHitMarker;

    private void Start()
    {
        Player.instance.onDealAttack += HandlePlayerDealtAttack;
        Enemy.instance.onDealAttack += HandleEnemyDealtAttack;
    }
    
    private void HandlePlayerDealtAttack(InfoAttackHit hit)
    {
        Instantiate(playerHitMarker, transform).hit = hit;
    }

    private void HandleEnemyDealtAttack(InfoAttackHit hit)
    {
        Instantiate(enemyHitMarker, transform).hit = hit;
    }
}
