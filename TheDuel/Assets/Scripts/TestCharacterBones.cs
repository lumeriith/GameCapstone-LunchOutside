using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class TestCharacterBones : SerializedMonoBehaviour
{
    public TestNode nodeTemplate;
    public Animator animator;
    public bool useLocalRotation = true;
    public Vector3 posOffset;
    
    private Dictionary<Transform, TestNode> _nodes;


    private void Start()
    {
        PrepareNodes();
    }

    private void PrepareNodes()
    {
        _nodes = new Dictionary<Transform, TestNode>();
        var vals = Enum.GetValues(typeof(HumanBodyBones));
        foreach (var v in vals)
        {
            var boneVal = (HumanBodyBones) v;
            if (boneVal >= HumanBodyBones.LeftThumbDistal && boneVal <= HumanBodyBones.RightLittleDistal) continue;
            var boneTransform = animator.GetBoneTransform(boneVal);
            if (boneTransform == null) continue;

            var newNode = Instantiate(nodeTemplate, transform);
            _nodes.Add(boneTransform, newNode);
            newNode.name = Enum.GetName(typeof(HumanBodyBones), boneVal);
        }
    }

    private void LateUpdate()
    {
        foreach (var pair in _nodes)
        {
            var bone = pair.Key;
            var node = pair.Value;
            if (bone == null || node == null) continue;
            node.transform.position = bone.position + posOffset;
            node.transform.rotation = useLocalRotation ? bone.localRotation : bone.rotation;
        }
    }
}
