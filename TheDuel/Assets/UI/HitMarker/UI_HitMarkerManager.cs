using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_HitMarkerManager : MonoBehaviour
{
    public UI_HitMarker playerValidHitMarker;
    public UI_HitMarker enemyValidHitMarker;
    
    public UI_HitMarker playerInvalidHitMarker;
    public UI_HitMarker enemyInvalidHitMarker;

    private void Start()
    {
        ScoreManager.instance.onPlayerValidAttack += (hit) => HandlePlayerDealtAttack(hit, true);
        ScoreManager.instance.onPlayerInvalidAttack += (hit) => HandlePlayerDealtAttack(hit, false);

        ScoreManager.instance.onEnemyValidAttack += (hit) => HandleEnemyDealtAttack(hit, true);
        ScoreManager.instance.onEnemyInvalidAttack += (hit) => HandleEnemyDealtAttack(hit, false);
    }
    
    private void HandlePlayerDealtAttack(InfoAttackHit hit, bool isValid)
    {
        Instantiate(isValid ? playerValidHitMarker : playerInvalidHitMarker, transform).hit = hit;
    }

    private void HandleEnemyDealtAttack(InfoAttackHit hit, bool isValid)
    {
        Instantiate(isValid ? enemyValidHitMarker : enemyInvalidHitMarker, transform).hit = hit;
    }
}
