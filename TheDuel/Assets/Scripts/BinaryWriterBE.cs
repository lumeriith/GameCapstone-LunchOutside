using System;
using System.IO;
using UnityEngine;

class BinaryWriterBE : BinaryWriter { 
    public BinaryWriterBE(System.IO.Stream stream)  : base(stream) { }

    public override void Write(int value)
    {
        var data = BitConverter.GetBytes(value);
        Array.Reverse(data);
        base.Write(data);
    }
}