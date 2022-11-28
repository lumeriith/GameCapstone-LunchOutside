using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StoneProjectile : Projectile
{
    public float stunDuration;
    public float headshotStunDuration;
    public string[] headshotGameObjectNames;
    public float minVelocity = 3f;

    private Rigidbody _rb;
    private bool isValid = true;

    protected override void Awake()
    {
        base.Awake();
        _rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isValid)
        {
            isValid = false;
            if (collision.relativeVelocity.magnitude < minVelocity) return;
            var character = collision.gameObject.GetComponentInParent<Character>();
            if (character == null) return;
            var hitName = collision.collider.name;
            var isHeadshot = headshotGameObjectNames.Contains(hitName);
            character.Stun(isHeadshot ? headshotStunDuration : stunDuration);
            if (isHeadshot) character.PlayHitHead();
            else character.PlayHitFront();
        }
    }
}
