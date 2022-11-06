using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModelComponentBase : SerializedMonoBehaviour
{
    public ModelSetupData setup => _dataReader.setup;
    public ModelReceivedData data => _dataReader.data;
    public ModelConnection connection => _connection;
    public ModelDataReader reader => _dataReader;
    public ModelDataWriter writer => _dataWriter;

    public bool isConnected => _connection.isConnected;

    private ModelConnection _connection;
    private ModelDataReader _dataReader;
    private ModelDataWriter _dataWriter;
    
    protected virtual void Start()
    {
        _connection = GetComponent<ModelConnection>();
        _dataReader = GetComponent<ModelDataReader>();
        _dataWriter = GetComponent<ModelDataWriter>();
        _dataReader.onSetupDataReceived += () =>
        {
            try
            {
                OnSetupDataReceived();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        };
        _dataReader.onDataChanged += () =>
        {
            if (!enabled) return;
            try
            {
                OnDataChanged();
            }
            catch (Exception e)
            {
                Debug.LogException(e, this);
            }
        };
    }

    protected virtual void OnSetupDataReceived()
    {
        
    }

    protected virtual void OnDataChanged()
    {
        
    }
}
