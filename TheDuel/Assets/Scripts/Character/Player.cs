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
        if (Input.GetKeyDown(KeyCode.Mouse0)) UseItem();
        var wheel = Input.GetAxis("Mouse ScrollWheel");
        if (wheel > 0)
        {
            CycleItemUp();
        }
        else if (wheel < 0)
        {
            CycleItemDown();
        }

        isAiming = Input.GetKey(KeyCode.Mouse1) && equippedItem != null && equippedItem is ThrowingItem;
        _trajectoryRenderer.enabled = isAiming;
        if (isAiming) UpdateTrajectory();
    }

    private void UpdateTrajectory()
    {
        const float TrajectoryDeltaTime = 0.06f;
        
        var weap = (ThrowingItem) equippedItem;

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
