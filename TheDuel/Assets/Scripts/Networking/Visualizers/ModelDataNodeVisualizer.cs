using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModelDataNodeVisualizer : ModelVisualizerBase
{
    public Vector3 globalOffset;
    public float visualizeScale = 0.02f;
    public bool keepAtCenter = true;
    public TestNode nodeTemplate;
    public bool useMatrices = false;
    public bool useParentalStructure = false;
    public bool invertMultiplyOrder = false;
    public Dictionary<string, string> parentInfo;

    
    private Dictionary<string, TestNode> _boneToNode;


    protected override void OnSetupDataReceived()
    {
        base.OnSetupDataReceived();
        
        _boneToNode = new Dictionary<string, TestNode>();
        
        foreach (var pair in setup.boneToPosIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
            newNode.transform.position = globalOffset;
        }

        foreach (var pair in setup.boneToRotIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
            newNode.transform.position = globalOffset;
        }

        foreach (var pair in setup.boneToMatIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
            newNode.transform.position = globalOffset;
        }
    }

    protected override void OnDataChanged()
    {
        base.OnDataChanged();
        Vector3 offset = Vector3.zero;
        if (keepAtCenter)
        {
            for (int i = 0; i < data.positions.Length; i++)
            {
                offset += data.positions[i];
            }

            offset /= data.positions.Length;
            offset.y = 0;
        }

        foreach (var pair in setup.boneToPosIndex)
        {
            var node = _boneToNode[pair.Key];
            node.transform.position = (data.positions[pair.Value] - offset) * visualizeScale + globalOffset;
        }
        
        foreach (var pair in setup.boneToRotIndex)
        {
            var node = _boneToNode[pair.Key];
            node.MakeRed();
            node.transform.rotation = GetRotation(pair.Key, false);
        }

        if (useMatrices)
        {
            foreach (var pair in setup.boneToMatIndex)
            {
                var node = _boneToNode[pair.Key];
                node.MakeYellow();
                node.transform.rotation = GetRotation(pair.Key, true);
            }
        }
    }

    private Quaternion GetRotation(string key, bool useMatrix)
    {
        Quaternion baseRot = parentInfo.ContainsKey(key) && useParentalStructure
            ? GetRotation(parentInfo[key], true)
            : Quaternion.identity;
        
        if (useMatrix)
        {
            var mat = data.matrices[setup.boneToMatIndex[key]];
            if (!mat.ValidTRS()) return Quaternion.identity;
            return invertMultiplyOrder ? mat.rotation * baseRot : baseRot * mat.rotation;
        }
        
        var rotVec = data.rotations[setup.boneToRotIndex[key]];
        Quaternion rot;
        var rotx = Quaternion.AngleAxis(rotVec.x, Vector3.right);
        var roty = Quaternion.AngleAxis(rotVec.y, Vector3.up);
        var rotz = Quaternion.AngleAxis(rotVec.z, Vector3.forward);
        rot =  rotz * rotx * roty;
        return invertMultiplyOrder ? rot * baseRot : baseRot * rot;
    }
}
