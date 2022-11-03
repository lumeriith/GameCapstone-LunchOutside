using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModelDataCharacterVisualizer : ModelVisualizerBase
{
    [Serializable]
    public class BoneItem
    {
        public string name;
        public HumanBodyBones bone;
        public Quaternion baseRot;
        public Quaternion afterRot;
    }

    public List<BoneItem> skeletonSettings;
    public Animator connectedCharacter;
    public bool setCharacterBonePos = false;
    public bool enableRetargeting;
    public bool useBaseRot;
    public bool useAfterRot;

    [Button]
    private void Build()
    {
        for (int i = 0; i < skeletonSettings.Count; i++)
        {
            var item = skeletonSettings[i];
            var characterBone = connectedCharacter.GetBoneTransform(item.bone);
            var modelMatrix = data.matrices[setup.boneToMatIndex[item.name]];
            var fromRotation = modelMatrix.rotation;
            var toRotation = characterBone.localRotation;
            
            // afterRot * fromRotation = toRotation;
            // OR fromRotation * baseRot = toRotation;
            item.afterRot = toRotation * Quaternion.Inverse(fromRotation);
            item.baseRot = Quaternion.Inverse(fromRotation) * toRotation;
        }
    }
    

    protected override void OnDataChanged()
    {
        base.OnDataChanged();
        if (!enableRetargeting) return;
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

                boneTransform.localRotation = mat.rotation;
                if (useBaseRot) boneTransform.localRotation = boneTransform.localRotation * pair.baseRot;
                if (useAfterRot)
                    boneTransform.localRotation = pair.afterRot * boneTransform.localRotation;
            }
        }
    }
}
