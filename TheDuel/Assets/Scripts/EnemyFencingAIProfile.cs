using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="EnemyAI", menuName="Enemy AI Profile")]
public class EnemyFencingAIProfile : ScriptableObject
{
    [Header("Look AI Settings")] 
    public float lookNoiseSpeed = 1;
    public float lookRotationMinOffset = 2;
    public float lookRotationMaxOffset = 10;

    [Header("Movement AI Settings")]
    public float horizontalNoiseSpeed = 1;
    public float horizontalStayCenterWeight = 1;
    public float horizontalRandomWeight = 1;
    public float verticalNoiseSpeed = 1;
    public float verticalRandomWeight = 3;
    public float desiredDistance = 5f;
    public float distanceDifferenceWeight = 1f;

    [Header("Initiative AI Settings")] 
    public float initiativeNoiseSpeed = 1;
    public float initiativeProximityMaxDistance = 4;
    public float initiativeProximityMinDistance = 2;
    public float initiativeProximityWeight = 2f;
    public float initiativeThreshold = 1.6f;
    public float initiativeRandomWeight = 1f;
    public float initiativeCooldown = 0.5f;
    public float basicAttackChance = 0.7f;
    public float leapAttackChance = 0.3f;
}
