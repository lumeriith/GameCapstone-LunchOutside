using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModelDataWriter : ModelComponentBase
{
    public enum OpCode : int
    {
        DoAction = 0,
        SetDirection = 1,
        SetTimescale = 2,
        SetTotalAgility = 3,
    }

    private float _lastActionTime = float.NegativeInfinity;

    public void WriteDoAction(int action)
    {
        WriteOpCode(OpCode.DoAction);
        connection.writer.Write(action);
        connection.FlushOutgoingPacket();
    }
    
    public void WriteSetDirection(Vector2 dir)
    {
        WriteOpCode(OpCode.SetDirection);
        connection.writer.Write(dir);
        connection.FlushOutgoingPacket();
    }

    public void WriteSetTimescale(float timescale)
    {
        WriteOpCode(OpCode.SetTimescale);
        connection.writer.Write(timescale);
        connection.FlushOutgoingPacket();
    }

    public void WriteSetTotalAgility(double totalAgility)
    {
        WriteOpCode(OpCode.SetTotalAgility);
        connection.writer.Write(totalAgility);
        connection.FlushOutgoingPacket();
    }

    public void WriteOpCode(OpCode op)
    {
        connection.writer.Write((int) op);
    }
}
