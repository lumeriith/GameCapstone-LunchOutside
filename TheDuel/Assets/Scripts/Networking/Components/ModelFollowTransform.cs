using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelFollowTransform : ModelComponentBase
{
    public float interval = 0.5f;
    public Transform from;
    public Transform target;

    private float _lastSendInterval;
    
    private void Update()
    {
        if (Time.time - _lastSendInterval > interval)
        {
            _lastSendInterval = Time.time;
            var diff = target.position - from.position;
            var dirVec = new Vector2(diff.x, diff.z).normalized;
            writer.WriteSetDirection(dirVec);
        }
    }
}
