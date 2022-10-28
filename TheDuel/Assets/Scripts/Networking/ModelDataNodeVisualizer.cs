using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModelDataNodeVisualizer : SerializedMonoBehaviour
{
    public float visualizeScale = 0.02f;
    public bool keepAtCenter = true;
    public TestNode nodeTemplate;
    public bool useMatrices = false;
    public bool useParentalStructure = false;
    public bool invertMultiplyOrder = false;
    public Dictionary<string, string> parentInfo;
    public bool showOrientationAsVector = true;
    
    private Dictionary<string, TestNode> _boneToNode;

    private ModelDataReader _dataReader;
    private ModelSetupData _setup => _dataReader.setup;
    private ModelReceivedData _data => _dataReader.data;
    
    private void Start()
    {
        _dataReader = GetComponent<ModelDataReader>();
        _dataReader.onSetupDataReceived += ProcessSetupData;
        _dataReader.onDataChanged += ProcessData;
    }

    private void ProcessSetupData()
    {
        _boneToNode = new Dictionary<string, TestNode>();
        
        foreach (var pair in _setup.boneToPosIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
        }

        foreach (var pair in _setup.boneToRotIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
        }

        foreach (var pair in _setup.boneToMatIndex)
        {
            if (_boneToNode.ContainsKey(pair.Key)) continue;
            var newNode = Instantiate(nodeTemplate, transform);
            _boneToNode.Add(pair.Key, newNode);
            newNode.name = pair.Key;
        }
    }
    
    private Quaternion GetRotation(string key, bool useMatrix)
    {
        Quaternion baseRot = parentInfo.ContainsKey(key) && useParentalStructure
            ? GetRotation(parentInfo[key], true)
            : Quaternion.identity;
        
        if (useMatrix)
        {
            var mat = _data.matrices[_setup.boneToMatIndex[key]];
            if (!mat.ValidTRS()) return Quaternion.identity;
            return invertMultiplyOrder ? mat.rotation * baseRot : baseRot * mat.rotation;
        }
        
        var rotVec = _data.rotations[_setup.boneToRotIndex[key]];
        Quaternion rot;
        if (showOrientationAsVector)
        {
            rot = Quaternion.LookRotation(rotVec);
        }
        else
        {
            var rotx = Quaternion.AngleAxis(rotVec.x, Vector3.right);
            var roty = Quaternion.AngleAxis(rotVec.y, Vector3.up);
            var rotz = Quaternion.AngleAxis(rotVec.z, Vector3.forward);
            rot =  rotz * rotx * roty;
        }
        return invertMultiplyOrder ? rot * baseRot : baseRot * rot;
    }
    
    private void ProcessData()
    {
        Vector3 offset = Vector3.zero;
        if (keepAtCenter)
        {
            for (int i = 0; i < _data.positions.Length; i++)
            {
                offset += _data.positions[i];
            }

            offset /= _data.positions.Length;
            offset.y = 0;
        }

        foreach (var pair in _setup.boneToPosIndex)
        {
            var node = _boneToNode[pair.Key];
            node.transform.position = (_data.positions[pair.Value] - offset) * visualizeScale;
        }
        
        foreach (var pair in _setup.boneToRotIndex)
        {
            var node = _boneToNode[pair.Key];
            node.MakeRed();
            node.transform.rotation = GetRotation(pair.Key, false);
        }

        if (useMatrices)
        {
            foreach (var pair in _setup.boneToMatIndex)
            {
                var node = _boneToNode[pair.Key];
                node.MakeYellow();
                node.transform.rotation = GetRotation(pair.Key, true);
            }
        }
    }
}
