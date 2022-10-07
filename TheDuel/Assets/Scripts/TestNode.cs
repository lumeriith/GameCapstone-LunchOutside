using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TestNode : MonoBehaviour
{
    public Material redMat;
    public Material yellowMat;

    private MeshRenderer _renderer;
    
    private void Awake()
    {
        _renderer = GetComponent<MeshRenderer>();
    }

    public void MakeRed()
    {
        _renderer.sharedMaterial = redMat;
    }

    public void MakeYellow()
    {
        _renderer.sharedMaterial = yellowMat;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, name);
    }
}
