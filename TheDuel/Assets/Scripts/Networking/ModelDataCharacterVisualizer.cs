using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDataCharacterVisualizer : MonoBehaviour
{
    [Serializable]
    public struct BoneItem
    {
        public string name;
        public HumanBodyBones bone;
        public Vector3 baseRot;
        public Vector3 afterRot;
    }
    
    public List<BoneItem> skeletonSettings;
    public Animator connectedCharacter;
    public bool setCharacterBonePos = false;
    
    private Dictionary<HumanBodyBones, Quaternion> _tPoseRots;
    
    private ModelDataReader _dataReader;
    private ModelSetupData _setup => _dataReader.setup;
    private ModelReceivedData _data => _dataReader.data;
    
    private void Start()
    {
        _dataReader = GetComponent<ModelDataReader>();
        _dataReader.onDataChanged += ApplyToCharacter;
    }

    private void ApplyToCharacter() 
    {
        if (_tPoseRots == null)
        {
            _tPoseRots = new Dictionary<HumanBodyBones, Quaternion>();
            for (int i = 0; i < (int)HumanBodyBones.LastBone; i++)
            {
                var enumVal = (HumanBodyBones) i;
                var bone = connectedCharacter.GetBoneTransform(enumVal);
                if (bone == null) continue;
                _tPoseRots.Add(enumVal, bone.localRotation);
            }
        }
        
        foreach (var pair in skeletonSettings)
        {
            var boneTransform = connectedCharacter.GetBoneTransform(pair.bone);
            if (boneTransform == null) continue;

            if (setCharacterBonePos)
            {
                if (_setup.boneToPosIndex.TryGetValue(pair.name, out var posIndex))
                {
                    var pos = _data.positions[posIndex];
                    boneTransform.position = pos;
                }
            }

            if (_setup.boneToMatIndex.TryGetValue(pair.name, out var matIndex))
            {
                var mat = _data.matrices[matIndex];
                boneTransform.localRotation = Quaternion.Euler(pair.afterRot) * mat.rotation * Quaternion.Euler(pair.baseRot);
            }
        }
    }
}
