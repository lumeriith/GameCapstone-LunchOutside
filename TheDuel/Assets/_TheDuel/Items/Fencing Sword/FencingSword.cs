using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct InfoAttackHit
{
    public Character from;
    public Character to;
    public Vector3 point;
    public Vector3 normal;
    public Collider collider;
    public int score;
}

public class FencingSword : Item
{
    public bool canParry;
    
    public Transform startPivot;
    public Transform endPivot;

    public Transform parryStartPivot;
    public Transform parryEndPivot;

    public int score = 1;
    public float sphereCastRadius = 0.075f;
    public float parrySphereCastRadius = 0.075f;

    public const float HitCooldown = 0.75f;

    public bool forceUseLeapAttack = false;

    public Effect swingEffect;
    public Effect leapSwingEffect;
    public Effect stabEffect;
    public Effect hitEffect;
    public Effect parryingEffect;
    public Effect hitBoxEffect;
    
    private Character _parent;
    private float _lastHitTime = Single.NegativeInfinity;

    protected override void Awake()
    {
        base.Awake();
        _parent = GetComponentInParent<Character>();
        _animator = _parent.GetComponent<Animator>();
    }

    protected override void Update()
    {
        base.Update();
        if (!isEquipped) return;
        
        if (_parent.isParrying && canParry)
        {
            var parryDistance = parryEndPivot.position - parryStartPivot.position;

            if (Physics.SphereCast(parryStartPivot.position, parrySphereCastRadius, parryDistance, out var parryHit, parryDistance.magnitude, LayerMask.GetMask("Parryable"),
                    QueryTriggerInteraction.Collide))
            {
                var hitCharacter = parryHit.collider.GetComponentInParent<Character>();
                var otherFencingSword = hitCharacter.GetComponentInChildren<FencingSword>();

                if (otherFencingSword != null && this != otherFencingSword && hitCharacter.isAttacking && _parent.isParrying)
                {
                    parryingEffect.Play();
                    hitCharacter.isAttacking = false;
                    hitCharacter.AddStamina(_parent.parryingDecreateStamina);
                    hitCharacter.PlayHitFront();
                    _lastHitTime = Time.time;
                }
            }
        }
        
        if (_parent.isAttacking)
        {
            var d = endPivot.position - startPivot.position;

            if (!Physics.SphereCast(startPivot.position, sphereCastRadius, d, out var hit, d.magnitude, LayerMask.GetMask("Attackable"),
                    QueryTriggerInteraction.Collide)) return;


            if (Time.time - _lastHitTime < HitCooldown)
            {
                return;
            }
        
            var otherCharacter = hit.collider.GetComponentInParent<Character>();
            if (otherCharacter != null && _parent != otherCharacter)
            {
                var hitInfo = new InfoAttackHit
                {
                    from = _parent,
                    to = otherCharacter,
                    point = hit.point,
                    normal = hit.normal,
                    collider = hit.collider,
                    score = score
                };
                _parent.onDealAttack?.Invoke(hitInfo);
                otherCharacter.onTakeAttack?.Invoke(hitInfo);
                stabEffect.PlayNew(hit.point, Quaternion.identity);
                hitEffect.PlayNew(hit.point, Quaternion.identity);
                _parent.isGetLastScore = true;
                _lastHitTime = Time.time;
            }
        }
    }
    
    private Animator _animator;

    public override void OnUse()
    {
        base.OnUse();
        var isLeapAttack = forceUseLeapAttack || (_parent == Player.instance && Input.GetKey(KeyCode.W));

        if (isLeapAttack)
        {
            leapSwingEffect.Play();
            _parent.PlayLeapAttack();
        }
        else
        {
            swingEffect.Play();
            if (_parent is Player)
            {
                var cols = Physics.OverlapSphere(_parent.transform.position + _parent.transform.forward * 2.5f, 1.5f,
                    LayerMask.GetMask("Attackable"));
                foreach (var c in cols)
                {
                    if (c.TryGetComponent<WeaponBox>(out var box))
                    {
                        _parent.PlayLowAttack();
                        StartCoroutine(HitWeaponBoxRoutine(box));
                        return;
                    }
                }
                _parent.PlayBasicAttack();
            }
            else
            {
                _parent.PlayBasicAttack();
            }
        }
    }

    private IEnumerator HitWeaponBoxRoutine(WeaponBox box)
    {
        hitBoxEffect.Play();
        yield return new WaitForSeconds(.5f);
        box.Hit();
    }
}
