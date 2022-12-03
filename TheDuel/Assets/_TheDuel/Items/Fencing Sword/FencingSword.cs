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

                if (otherFencingSword != null && this != otherFencingSword && hitCharacter.isAttacking)
                {
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
            var otherWeaponBox = hit.collider.GetComponent<WeaponBox>();
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
                _lastHitTime = Time.time;
            }

            if (otherWeaponBox != null)
            {
                otherWeaponBox.Hit();
                _lastHitTime = Time.time;
            }
        }
    }
    
    private Animator _animator;

    public override void OnUse()
    {
        base.OnUse();
        var isLeapAttack = forceUseLeapAttack || (_parent == Player.instance && Input.GetKey(KeyCode.W));

        if (isLeapAttack) _parent.PlayLeapAttack();
        else _parent.PlayBasicAttack();
    }
}
