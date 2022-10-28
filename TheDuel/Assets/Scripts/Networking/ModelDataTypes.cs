using System.Collections.Generic;
using UnityEngine;

public struct ModelReceivedData
{
    public Vector3[] positions;
    public Vector3[] rotations;
    public Matrix4x4[] matrices;
}

public struct ModelSetupData
{
    public Dictionary<int, string> posIndexToBone;
    public Dictionary<string, int> boneToPosIndex;
        
    public Dictionary<int, string> rotIndexToBone;
    public Dictionary<string, int> boneToRotIndex;
        
    public Dictionary<int, string> matIndexToBone;
    public Dictionary<string, int> boneToMatIndex;
        
    public int numOfPosJoints;
    public int numOfRotJoints;
    public int numOfMatrices;
}