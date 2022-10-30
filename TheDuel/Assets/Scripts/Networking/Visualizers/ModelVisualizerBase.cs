using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class ModelVisualizerBase : SerializedMonoBehaviour
{
    public ModelSetupData setup => _dataReader.setup;
    public ModelReceivedData data => _dataReader.data;
    public bool isConnected => _connection.isConnected;

    private ModelConnection _connection;
    private ModelDataReader _dataReader;
    
    protected virtual void Start()
    {
        _connection = GetComponent<ModelConnection>();
        _dataReader = GetComponent<ModelDataReader>();
        _dataReader.onSetupDataReceived += OnSetupDataReceived;
        _dataReader.onDataChanged += () =>
        {
            if (!enabled) return;
            OnDataChanged();
        };
    }

    protected virtual void OnSetupDataReceived()
    {
        
    }

    protected virtual void OnDataChanged()
    {
        
    }
}
