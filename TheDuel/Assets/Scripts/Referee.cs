using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Referee : MonoBehaviour
{
    public static Referee instance
    {
        get
        {
            if (_instance == null) _instance = FindObjectOfType<Referee>();
            return _instance;
        }
    }
    private static Referee _instance;

    public Action<float, float> onSuspicionChanged;
    public Action<float, float> onVisibilityChanged;

    public Transform visionStartPoint;
    public float visionAngle;
    public List<HumanBodyBones> sampleBones;
    public float suspicionGrowSpeedMin = 0.1f;
    public float suspicionGrowSpeedMax = 0.5f;
    public float suspicionDecaySpeed = 0.5f;

    
    public float currentSuspicion { get; private set; }
    public float currentVisibility { get; private set; }
    
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

    private void FixedUpdate()
    {
        var prevVis = currentVisibility;
        
        var startPoint = visionStartPoint.position;
        int visibleRays = 0;
        foreach (var t in _sampleTransforms)
        {
            var endPoint = t.position;
            var delta = endPoint - startPoint;
            if (Vector3.Angle(delta, visionStartPoint.forward) > visionAngle) continue;
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
        currentVisibility = (float)visibleRays / _sampleTransforms.Length;
        
        if (currentVisibility != prevVis)
        {
            onVisibilityChanged(prevVis, currentVisibility);
        }
    }

    private void Update()
    {
        var prevSus = currentSuspicion;
        if (currentVisibility > 0 && Player.instance.isCheating)
        {
            var speed = Mathf.Lerp(suspicionGrowSpeedMin, suspicionGrowSpeedMax, currentVisibility);
            currentSuspicion = Mathf.MoveTowards(prevSus, 1, speed * Time.deltaTime);
        }
        else
        {
            currentSuspicion = Mathf.MoveTowards(prevSus, 0, suspicionDecaySpeed * Time.deltaTime);
        }

        if (currentSuspicion != prevSus)
        {
            onSuspicionChanged(prevSus, currentSuspicion);
        }
    }
}
