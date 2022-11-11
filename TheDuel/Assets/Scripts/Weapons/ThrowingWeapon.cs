using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingWeapon : Weapon
{
    public GameObject projectilePrefab;
    public Vector3 throwVelocity;
    
    

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        if (!isEquipped) return;
    }
}
