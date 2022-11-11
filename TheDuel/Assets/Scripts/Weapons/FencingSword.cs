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

public class FencingSword : Weapon
{
    public Transform startPivot;
    public Transform endPivot;
    public int score = 1;

    public const float HitCooldown = 0.75f;
    
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
        var d = endPivot.position - startPivot.position;
        
        if (!Physics.Raycast(startPivot.position, d, out var hit, d.magnitude, LayerMask.GetMask("Character Hitbox"),
                QueryTriggerInteraction.Collide)) return;
        var other = hit.collider.GetComponentInParent<Character>();
        if (other == null) return;
        
        if (Time.time - _lastHitTime < HitCooldown)
        {
            _lastHitTime = Time.time;
            return;
        }
        var hitInfo = new InfoAttackHit
        {
            from = _parent,
            to = other,
            point = hit.point,
            normal = hit.normal,
            collider = hit.collider,
            score = score
        };
        _parent.onDealAttack?.Invoke(hitInfo);
        other.onTakeAttack?.Invoke(hitInfo);
        _lastHitTime = Time.time;
    }
    
    private Animator _animator;

    public override void OnUse()
    {
        base.OnUse();
        if (Input.GetKey(KeyCode.W)) _animator.SetTrigger("Leap Attack");
        else _animator.SetTrigger("Basic Attack");
    }
}
