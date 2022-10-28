using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModelActionSender : MonoBehaviour
{
    public enum OpCode : int
    {
        DoAction = 0,
        SetDirection = 1,
    }

    public bool repeatAction = false;
    public int actionNumber = 3;
    public float repeatInterval = 1f;

    private float _lastActionTime = float.NegativeInfinity;
    
    private ModelConnection _connection;

    private void Start()
    {
        _connection = GetComponent<ModelConnection>();
    }

    private void Update()
    {
        if (_connection.isConnected)
        {
            if (Input.GetKeyDown(KeyCode.Space)) DoActionTest();
            if (Input.GetKeyDown(KeyCode.Q)) SetDirection(Random.insideUnitCircle);
            if (Input.GetKeyDown(KeyCode.W)) SetDirection(Vector2.zero);

            if (Time.time - _lastActionTime > repeatInterval && repeatAction)
            {
                _lastActionTime = Time.time;
                DoAction(actionNumber);
            }
        }
    }

    private int _actionNumber = 0;
    private void DoActionTest()
    {
        WriteOpCode(OpCode.DoAction);
        if (_actionNumber == 5) _actionNumber = 0;
        _connection.writer.Write(_actionNumber++);
        _connection.FlushOutgoingPacket();
    }
    
    private void DoAction(int action)
    {
        WriteOpCode(OpCode.DoAction);
        _connection.writer.Write(action);
        _connection.FlushOutgoingPacket();
    }
    
    private void SetDirection(Vector2 dir)
    {
        WriteOpCode(OpCode.SetDirection);
        _connection.writer.Write(dir);
        _connection.FlushOutgoingPacket();
    }

    private void WriteOpCode(OpCode op)
    {
        _connection.writer.Write((int) op);
    }
}
