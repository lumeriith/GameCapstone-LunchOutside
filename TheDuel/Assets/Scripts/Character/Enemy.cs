using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Character
{
    public static Enemy instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Enemy>();
            return _instance;
        }
    }
    private static Enemy _instance;

    private OpenSimplexNoise _noiseGen;

    public float rotationSpeed;

    public bool disableAi;
    public EnemyAIProfile aiProfile;
    public bool disableInitiative;

    private float _noiseMovementVertical;
    private float _noiseMovementHorizontal;
    private float _noiseInitiative;
    private float _noiseLook;

    private float _lastInitiativeTime;


    protected override void Awake()
    {
        base.Awake();
        _noiseGen = new OpenSimplexNoise();
    }

    protected override void Update()
    {
        base.Update();
        if (!GameManager.instance.isRoundOngoing) return;
        if (disableAi) return;
        if (isStunned) return;
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Strafing Movement")) return;
        UpdateNoiseValues();
        
        var lookOffset = Mathf.Lerp(aiProfile.lookRotationMinOffset, aiProfile.lookRotationMaxOffset, _noiseLook / 2f + 0.5f);
        var targetRotation = Quaternion.Euler(0, Quaternion.LookRotation(Player.instance.transform.position - transform.position).eulerAngles.y + lookOffset, 0);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        var isBackward = Vector3.Angle(transform.forward, Vector3.forward) > 90;

        var desiredDistWeight = (Vector3.Distance(transform.position, Player.instance.transform.position) - aiProfile.desiredDistance) * aiProfile.distanceDifferenceWeight;
        
        var vMove = _noiseMovementVertical * aiProfile.verticalRandomWeight + desiredDistWeight;
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
        var hMove = _noiseMovementHorizontal * aiProfile.horizontalRandomWeight + hOffset * aiProfile.horizontalStayCenterWeight;
        hMove = Mathf.Clamp(hMove, -1, 1);
        
        animator.SetFloat("InputMagnitude", Mathf.Clamp(Mathf.Sqrt(vMove*vMove + hMove*hMove), 0, 1));
        animator.SetFloat("InputVertical", vMove);
        animator.SetFloat("InputHorizontal", hMove);
        
        if (disableInitiative) return;
        
        if (GetInitiativeValue() > aiProfile.initiativeThreshold && 
            Time.time - _lastInitiativeTime > aiProfile.initiativeCooldown)
        {
            _lastInitiativeTime = Time.time;
            var val = Random.value;
            if (val < aiProfile.basicAttackChance)
            {
                animator.SetTrigger("Basic Attack");
            } 
            else if (val < aiProfile.basicAttackChance + aiProfile.leapAttackChance)
            {
                animator.SetTrigger("Leap Attack");
            }
        }
    }

    private float GetInitiativeValue()
    {
        var dist = Vector3.Distance(Player.instance.transform.position, transform.position);
        var proximityWeight = Mathf.Lerp(aiProfile.initiativeProximityWeight, 0,
            (dist - aiProfile.initiativeProximityMinDistance) /
            (aiProfile.initiativeProximityMaxDistance - aiProfile.initiativeProximityMinDistance));
        
        return _noiseInitiative * aiProfile.initiativeRandomWeight + proximityWeight;
    }

    private void UpdateNoiseValues()
    {
        _noiseMovementVertical = _noiseGen.Evaluate(0, Time.time * aiProfile.verticalNoiseSpeed);
        _noiseMovementHorizontal = _noiseGen.Evaluate(100, Time.time * aiProfile.horizontalNoiseSpeed);
        _noiseInitiative = _noiseGen.Evaluate(200, Time.time * aiProfile.initiativeNoiseSpeed);
        _noiseLook = _noiseGen.Evaluate(300, Time.time * aiProfile.lookNoiseSpeed);
    }
}
