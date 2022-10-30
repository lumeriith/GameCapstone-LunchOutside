using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDataCharacterVisualizer : ModelVisualizerBase
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

    protected override void OnDataChanged()
    {
        base.OnDataChanged();
        
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
                if (setup.boneToPosIndex.TryGetValue(pair.name, out var posIndex))
                {
                    var pos = data.positions[posIndex];
                    boneTransform.position = pos;
                }
            }

            if (setup.boneToMatIndex.TryGetValue(pair.name, out var matIndex))
            {
                var mat = data.matrices[matIndex];
                boneTransform.localRotation = Quaternion.Euler(pair.afterRot) * mat.rotation * Quaternion.Euler(pair.baseRot);
            }
        }
    }
}
