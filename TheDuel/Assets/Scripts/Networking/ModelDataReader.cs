using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelDataReader : MonoBehaviour
{
    public ModelReceivedData data => _data;
    public ModelSetupData setup => _setup;

    public Action onSetupDataReceived;
    public Action onDataChanged;
    
    private ModelReceivedData _data = new ModelReceivedData();
    private ModelSetupData _setup = new ModelSetupData();
    
    private ModelConnection _connection;
    
    private bool _isFirstPayload = true;

    private void Start()
    {
        _connection = GetComponent<ModelConnection>();
        _connection.dataReader = ReadData;
    }

    private void ReadData(BinaryReaderBE reader)
    {
        if (_isFirstPayload)
        {
            _setup.numOfPosJoints = reader.ReadInt32();
            _setup.boneToPosIndex = new Dictionary<string, int>();
            _setup.posIndexToBone = new Dictionary<int, string>();
            _data.positions = new Vector3[_setup.numOfPosJoints];

            
            for (int i = 0; i < _setup.numOfPosJoints; i++)
            {
                string jointName = reader.ReadString();
                _setup.boneToPosIndex.Add(jointName, i);
                _setup.posIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfRotJoints = reader.ReadInt32();
            _setup.boneToRotIndex = new Dictionary<string, int>();
            _setup.rotIndexToBone = new Dictionary<int, string>();
            _data.rotations = new Vector3[_setup.numOfRotJoints];

            for (int i = 0; i < _setup.numOfRotJoints; i++)
            {
                string jointName = reader.ReadString();
                _setup.boneToRotIndex.Add(jointName, i);
                _setup.rotIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfMatrices = reader.ReadInt32();
            _setup.boneToMatIndex = new Dictionary<string, int>();
            _setup.matIndexToBone = new Dictionary<int, string>();
            _data.matrices = new Matrix4x4[_setup.numOfMatrices];

            for (int i = 0; i < _setup.numOfMatrices; i++)
            {
                string jointName = reader.ReadString();
                _setup.boneToMatIndex.Add(jointName, i);
                _setup.matIndexToBone.Add(i, jointName);
            }

            List<Joint> joints = new List<Joint>();
            int numOfJoints = reader.ReadInt32();
            for (int i = 0; i < numOfJoints; i++)
            {
                var newJoint = new Joint();
                newJoint.name = reader.ReadString();
                newJoint.translation = reader.ReadVector3();
                var parentIndex = reader.ReadInt32();
                if (parentIndex >= 0)
                {
                    joints[parentIndex].children.Add(newJoint);
                }
                joints.Add(newJoint);
            }

            _setup.root = joints[0];

            onSetupDataReceived?.Invoke();
            _isFirstPayload = false;
        }
        
        for (int i = 0; i < _setup.numOfPosJoints; i++)
        {
            _data.positions[i] = reader.ReadVector3();
        }
        
        for (int i = 0; i < _setup.numOfRotJoints; i++)
        {
            _data.rotations[i] = reader.ReadVector3();
        }

        for (int i = 0; i < _setup.numOfMatrices; i++)
        {
            _data.matrices[i] = reader.ReadMatrix4x4().transpose;
        }

        onDataChanged?.Invoke();
    }
}
