using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : Character
{
    public static Player instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Player>();
            return _instance;
        }
    }
    private static Player _instance;

    private LineRenderer _trajectoryRenderer;
    private Camera _cam;

    protected override void Awake()
    {
        base.Awake();
        _trajectoryRenderer = GetComponent<LineRenderer>();
        _trajectoryRenderer.positionCount = 20;
    }
    
    protected override void Start()
    {
        base.Start();
        _cam = Camera.main;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) UseWeapon();
        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            CycleWeaponUp();
        }
        else if (wheel < 0)
        {
            CycleWeaponDown();
        }

        isAiming = Input.GetKey(KeyCode.Mouse1) && equippedWeapon != null && equippedWeapon is ThrowingWeapon;
        _trajectoryRenderer.enabled = isAiming;
        if (isAiming) UpdateTrajectory();
    }

    private void UpdateTrajectory()
    {
        const float TrajectoryDeltaTime = 0.06f;
        
        var weap = (ThrowingWeapon) equippedWeapon;

        var pos = weap.transform.position;
        var vel = _cam.transform.rotation * weap.throwVelocity;
        for (int i = 0; i < _trajectoryRenderer.positionCount; i++)
        {
            _trajectoryRenderer.SetPosition(i, pos);
            pos += vel * TrajectoryDeltaTime;
            vel += Physics.gravity * TrajectoryDeltaTime;
        }
    }
}
