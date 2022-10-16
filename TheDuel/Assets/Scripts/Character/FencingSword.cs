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
}

public class FencingSword : MonoBehaviour
{
    public Transform startPivot;
    public Transform endPivot;

    public float cooldown = 1.5f;
    
    private Character _parent;
    private float _lastHitTime = Single.NegativeInfinity;
    
    private void Awake()
    {
        _parent = GetComponentInParent<Character>();
    }

    private void Update()
    {
        var d = endPivot.position - startPivot.position;
        
        if (!Physics.Raycast(startPivot.position, d, out var hit, d.magnitude, LayerMask.GetMask("Character Hitbox"),
                QueryTriggerInteraction.Collide)) return;
        var other = hit.collider.GetComponentInParent<Character>();
        if (other == null) return;
        
        if (Time.time - _lastHitTime < cooldown)
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
            collider = hit.collider
        };
        _parent.onDealAttack?.Invoke(hitInfo);
        other.onTakeAttack?.Invoke(hitInfo);
        _lastHitTime = Time.time;
    }
}
