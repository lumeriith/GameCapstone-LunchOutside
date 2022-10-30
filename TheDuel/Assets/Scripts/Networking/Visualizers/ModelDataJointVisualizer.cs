using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDataJointVisualizer : ModelVisualizerBase
{
    private const string RootNodeName = "Hips";
    
    public TestNode nodeTemplate;
    public Vector3 worldOffset;
    public float bodyScale = 0.1f;
    public float translationScale = 0.01f;
    public List<string> positionBone = new List<string>();

    public bool centerModelOnStart;
    
    private Dictionary<string, TestNode> _nodeByJointName = new Dictionary<string, TestNode>();
    private TestNode _root;
    private bool _didSetCenter;
    
    protected override void OnSetupDataReceived()
    {
        base.OnSetupDataReceived();
        CreateNode(setup.root, null);
    }

    private void CreateNode(Joint joint, TestNode parentNode)
    {
        var newNode = Instantiate(nodeTemplate, transform);
        newNode.name = joint.name;
        
        if (parentNode == null)
        {
            newNode.transform.localPosition += worldOffset;
            _root = newNode;
        }
        else
        {
            newNode.transform.parent = parentNode.transform;
            newNode.transform.localPosition = joint.translation * bodyScale;
        }
        
        _nodeByJointName.Add(joint.name, newNode);
        foreach (var c in joint.children)
        {
            CreateNode(c, newNode);
        }
    }

    private void DrawLinesToChildren(Transform root)
    {
        foreach (Transform c in root)
        {
            Debug.DrawLine(root.transform.position, c.position, Color.red, 1 / 30f);
            DrawLinesToChildren(c);
        }
    }
    
    protected override void OnDataChanged()
    {
        base.OnDataChanged();

        foreach (var pair in setup.boneToMatIndex)
        {
            var m = data.matrices[pair.Value];
            var node = _nodeByJointName[pair.Key];
            node.transform.localRotation = m.rotation;
            if (pair.Key == RootNodeName)
            {
                var translation = new Vector3(m.m03, m.m13, m.m23) * translationScale;
                if (centerModelOnStart && !_didSetCenter)
                {
                    var flat = translation;
                    flat.y = 0;
                    _didSetCenter = true;
                    worldOffset -= flat;
                }
                
                node.transform.position = translation + worldOffset;
            }
        }

        foreach (var n in positionBone)
        {
            if (!setup.boneToPosIndex.TryGetValue(n, out var posIndex))
            {
                Debug.LogWarning($"Pos index for {n} not found");
                continue;
            }

            if (!_nodeByJointName.TryGetValue(n, out var node))
            {
                Debug.LogWarning($"Node for {n} not found");
                continue;
            }
            node.transform.position = worldOffset + data.positions[posIndex] * bodyScale;
        }

        DrawLinesToChildren(_root.transform);
    }


}
