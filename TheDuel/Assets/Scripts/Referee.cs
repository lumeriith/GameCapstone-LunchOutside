using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    public Transform visionStartPoint;
    public float visionAngle;
    public List<HumanBodyBones> sampleBones;

    public float playerVisibility { get; private set; }
    
    private Transform[] _sampleTransforms;

    private void Start()
    {
        var anim = Player.instance.GetComponentInChildren<Animator>();
        _sampleTransforms = new Transform[sampleBones.Count];
        for (int i = 0; i < sampleBones.Count; i++)
        {
            _sampleTransforms[i] = anim.GetBoneTransform(sampleBones[i]);
        }
    }

    private void Update()
    {
        var startPoint = visionStartPoint.position;
        int visibleRays = 0;
        foreach (var t in _sampleTransforms)
        {
            var endPoint = t.position;
            var delta = endPoint - startPoint;
            if (Physics.Raycast(startPoint, delta, out var hit, delta.magnitude) && hit.collider.gameObject.GetComponentInParent<Player>() != null)
            {
                Debug.DrawLine(startPoint, endPoint, Color.green);
                visibleRays++;
            }
            else
            {
                Debug.DrawLine(startPoint, endPoint, Color.red);
            }
        }
        playerVisibility = (float)visibleRays / _sampleTransforms.Length;
    }
}
