using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDataJointVisualizer : ModelVisualizerBase
{
    public TestNode nodeTemplate;
    public Vector3 worldOffset;
    public float scale;
    
    private Dictionary<string, TestNode> _nodeByJointName = new Dictionary<string, TestNode>();
    private TestNode _root;
    
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
            newNode.transform.localPosition = joint.translation * scale;
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
            _nodeByJointName[pair.Key].transform.localRotation = data.matrices[pair.Value].rotation;
        }

        DrawLinesToChildren(_root.transform);
    }


}
