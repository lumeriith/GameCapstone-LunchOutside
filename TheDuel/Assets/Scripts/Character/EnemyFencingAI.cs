using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFencingAI : MonoBehaviour
{
    public EnemyFencingAIProfile profile;
    public bool disableInitiative;

    private float _noiseMovementVertical;
    private float _noiseMovementHorizontal;
    private float _noiseInitiative;
    private float _noiseLook;

    private float _lastInitiativeTime;
    
    private OpenSimplexNoise _noiseGen;
    private Enemy _enemy;

    public float rotationSpeed;

    private void Awake()
    {
        _noiseGen = new OpenSimplexNoise();
    }

    private void Start()
    {
        _enemy = GetComponent<Enemy>();
    }

    private void Update()
    { 
        if (!GameManager.instance.isRoundOngoing) return;
        if (_enemy.isStunned) return;
        if (!_enemy.animator.GetCurrentAnimatorStateInfo(0).IsName("Strafing Movement")) return;
        UpdateNoiseValues();
        
        var lookOffset = Mathf.Lerp(profile.lookRotationMinOffset, profile.lookRotationMaxOffset, _noiseLook / 2f + 0.5f);
        var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(Player.instance.transform.position - transform.position).eulerAngles.y + lookOffset, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        var isBackward = Vector3.Angle(transform.forward, Vector3.forward) > 90;

        var desiredDistWeight = (Vector3.Distance(transform.position, Player.instance.transform.position) - profile.desiredDistance) * profile.distanceDifferenceWeight;
        
        var vMove = _noiseMovementVertical * profile.verticalRandomWeight + desiredDistWeight;
        vMove = Mathf.Clamp(vMove, -1, 1);

        if (transform.position.z < GameManager.instance.playRegion.min.z + 2f)
        {
            if (!isBackward) vMove = Mathf.Clamp(vMove, 0, 1);
            else vMove = Mathf.Clamp(vMove, -1, 0);
        }
        else if (transform.position.z > GameManager.instance.playRegion.max.z - 2f)
        {
            if (isBackward) vMove = Mathf.Clamp(vMove, 0, 1);
            else vMove = Mathf.Clamp(vMove, -1, 0);
        }

        var hOffset = GameManager.instance.playRegion.center.x - transform.position.x;
        if (isBackward) hOffset *= -1;
        var hMove = _noiseMovementHorizontal * profile.horizontalRandomWeight + hOffset * profile.horizontalStayCenterWeight;
        hMove = Mathf.Clamp(hMove, -1, 1);
        
        _enemy.animator.SetFloat("InputMagnitude", Mathf.Clamp(Mathf.Sqrt(vMove*vMove + hMove*hMove), 0, 1));
        _enemy.animator.SetFloat("InputVertical", vMove);
        _enemy.animator.SetFloat("InputHorizontal", hMove);
        
        if (disableInitiative) return;
        
        if (GetInitiativeValue() > profile.initiativeThreshold && 
            Time.time - _lastInitiativeTime > profile.initiativeCooldown)
        {
            _lastInitiativeTime = Time.time;
            var val = UnityEngine.Random.value;
            if (val < profile.basicAttackChance)
            {
                _enemy.animator.SetTrigger("Basic Attack");
            } 
            else if (val < profile.basicAttackChance + profile.leapAttackChance)
            {
                _enemy.animator.SetTrigger("Leap Attack");
            }
        }
    }
    
    private float GetInitiativeValue()
    {
        var dist = Vector3.Distance(Player.instance.transform.position, transform.position);
        var proximityWeight = Mathf.Lerp(profile.initiativeProximityWeight, 0,
            (dist - profile.initiativeProximityMinDistance) /
            (profile.initiativeProximityMaxDistance - profile.initiativeProximityMinDistance));
        
        return _noiseInitiative * profile.initiativeRandomWeight + proximityWeight;
    }

    private void UpdateNoiseValues()
    {
        _noiseMovementVertical = _noiseGen.Evaluate(0, Time.time * profile.verticalNoiseSpeed);
        _noiseMovementHorizontal = _noiseGen.Evaluate(100, Time.time * profile.horizontalNoiseSpeed);
        _noiseInitiative = _noiseGen.Evaluate(200, Time.time * profile.initiativeNoiseSpeed);
        _noiseLook = _noiseGen.Evaluate(300, Time.time * profile.lookNoiseSpeed);
    }
}
