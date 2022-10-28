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
    
    private ModelReceivedData _data;
    private ModelSetupData _setup;
    
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

            char[] arr = new char[255];
            for (int i = 0; i < _setup.numOfPosJoints; i++)
            {
                int length = reader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = reader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToPosIndex.Add(jointName, i);
                _setup.posIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfRotJoints = reader.ReadInt32();
            _setup.boneToRotIndex = new Dictionary<string, int>();
            _setup.rotIndexToBone = new Dictionary<int, string>();
            _data.rotations = new Vector3[_setup.numOfRotJoints];

            for (int i = 0; i < _setup.numOfRotJoints; i++)
            {
                int length = reader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = reader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToRotIndex.Add(jointName, i);
                _setup.rotIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfMatrices = reader.ReadInt32();
            _setup.boneToMatIndex = new Dictionary<string, int>();
            _setup.matIndexToBone = new Dictionary<int, string>();
            _data.matrices = new Matrix4x4[_setup.numOfMatrices];

            for (int i = 0; i < _setup.numOfMatrices; i++)
            {
                int length = reader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = reader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToMatIndex.Add(jointName, i);
                _setup.matIndexToBone.Add(i, jointName);
            }

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
            _data.matrices[i] = reader.ReadMatrix4x4();
        }

        onDataChanged?.Invoke();
    }
}
