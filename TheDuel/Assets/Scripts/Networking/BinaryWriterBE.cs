using System;
using System.IO;
using UnityEngine;

public class BinaryWriterBE : BinaryWriter { 
    public BinaryWriterBE(System.IO.Stream stream)  : base(stream) { }

    public override void Write(int value)
    {
        var data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        base.Write(data);
    }

    public override void Write(float value)
    {
        var data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        base.Write(data);
    }

    public void Write(Vector2 value)
    {
        Write(value.x);
        Write(value.y);
    }
    
    public void Write(Vector3 value)
    {
        Write(value.x);
        Write(value.y);
        Write(value.z);
    }
}