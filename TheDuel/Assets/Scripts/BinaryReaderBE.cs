using System;
using System.IO;
using UnityEngine;

class BinaryReaderBE : BinaryReader { 
    public BinaryReaderBE(System.IO.Stream stream)  : base(stream) { }

    public Vector3 ReadVector3()
    {
        return new Vector3(ReadSingle(), ReadSingle(), ReadSingle());
    }  
    
    public override int ReadInt32()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToInt32(data, 0);
    }

    public override short ReadInt16()
    {
        var data = base.ReadBytes(2);
        Array.Reverse(data);
        return BitConverter.ToInt16(data, 0);
    }


    public override float ReadSingle()
    {
        var data = base.ReadBytes(4);
        Array.Reverse(data);
        return BitConverter.ToSingle(data, 0);
    }
    
    public override double ReadDouble()
    {
        var data = base.ReadBytes(8);
        Array.Reverse(data);
        return BitConverter.ToDouble(data, 0);
    }
}

