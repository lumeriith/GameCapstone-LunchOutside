using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class MrlSocketTest : SerializedMonoBehaviour
{
    public int dataBufferSize = 1024 * 300;
    public string host = "127.0.0.1";
    public int port = 1369;
    public float visualizeScale = 0.02f;
    public bool keepAtCenter = true;
    public TestNode nodeTemplate;
    public int nodeCount = 50;
    
    public Dictionary<string, HumanBodyBones> skeletonSettings;


    private List<TestNode> _nodes;

    private TcpClient _client;

    private NetworkStream _networkStream;
    private byte[] _networkStreamBuffer;
    
    private MemoryStream _incomingStream;
    private MemoryStream _outgoingStream;
    private BinaryReaderBE _incomingReader;
    private BinaryWriterBE _outgoingWriter;
    
    private byte[] _copyRemainBuffer;
    private bool _isFirstPayload = true;

    private void Start()
    {
        PrepareNodes();
        _client = new TcpClient
        {
            ReceiveBufferSize = dataBufferSize,
            SendBufferSize = dataBufferSize
        };
        _client.BeginConnect(host, port, asyncResult =>
        {
            _client.EndConnect(asyncResult);
            if (!_client.Connected)
            {
                Debug.LogWarning("Could not connect to model.");
                return;
            }
            Debug.Log("Connected to model.");
            _networkStream = _client.GetStream();
            _incomingStream = new MemoryStream();
            _incomingReader = new BinaryReaderBE(_incomingStream);
            _outgoingStream = new MemoryStream();
            _outgoingWriter = new BinaryWriterBE(_outgoingStream);
            _networkStreamBuffer = new byte[dataBufferSize];
            _copyRemainBuffer = new byte[dataBufferSize];
            _networkStream.BeginRead(_networkStreamBuffer, 0, dataBufferSize, PutToMemStream, null);
        }, _client);
    }

    private void PrepareNodes()
    {
        _nodes = new List<TestNode>();
        for (int i = 0; i < nodeCount; i++)
        {
            _nodes.Add(Instantiate(nodeTemplate, transform));
            _nodes[i].gameObject.SetActive(false);
            _nodes[i].name = "n" + i;
        }

        Destroy(nodeTemplate.gameObject);
    }

    private void PutToMemStream(IAsyncResult ar)
    {
        if (!_networkStream.CanRead) return;
        int bytesRead = _networkStream.EndRead(ar);
        if (bytesRead <= 0)
        {
            _client.Close();
            return;
        }

        lock (_incomingStream)
        {
            _incomingStream.Position = _incomingStream.Length;
            _incomingStream.Write(_networkStreamBuffer, 0, bytesRead);
        }
        _networkStream.BeginRead(_networkStreamBuffer, 0, dataBufferSize, PutToMemStream, null);
    }

    struct ReceivedData
    {
        public Vector3[] positions;
        public Vector3[] rotations;
        public Matrix4x4[] matrices;
    }

    struct SetupData
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

    private ReceivedData _data;
    private SetupData _setup;

    private void ProcessData()
    {
        Vector3 offset = Vector3.zero;
        if (keepAtCenter)
        {
            for (int i = 0; i < _data.positions.Length; i++)
            {
                offset += _data.positions[i];
            }

            offset /= _data.positions.Length;
            offset.y = 0;
        }
        for (int i = 0; i < _data.positions.Length; i++)
        {
            _nodes[i].name = _setup.posIndexToBone[i];
            _nodes[i].transform.position = (_data.positions[i] - offset) * visualizeScale;
            _nodes[i].gameObject.SetActive(true);

            if (_setup.boneToRotIndex.TryGetValue(_setup.posIndexToBone[i], out var rotIndex))
            {
                var rotVec = _data.rotations[rotIndex];
                
                var rot = Quaternion.AngleAxis(rotVec.y, Vector3.up) * Quaternion.AngleAxis(rotVec.x, Vector3.right) * Quaternion.AngleAxis(rotVec.z, Vector3.forward);
                _nodes[i].transform.rotation = rot;
                
                //_nodes[i].transform.rotation = Quaternion.Euler(_data.rotations[rotIndex]);
                _nodes[i].MakeRed();
            }

            if (_setup.boneToMatIndex.TryGetValue(_setup.posIndexToBone[i], out var matIndex))
            {
                var mat = _data.matrices[matIndex];
                _nodes[i].transform.rotation = mat.rotation;
            }
        }
    }
    
    private void ReadData()
    {
        if (_isFirstPayload)
        {
            _setup.numOfPosJoints = _incomingReader.ReadInt32();
            _setup.boneToPosIndex = new Dictionary<string, int>();
            _setup.posIndexToBone = new Dictionary<int, string>();
            _data.positions = new Vector3[_setup.numOfPosJoints];

            char[] arr = new char[255];
            for (int i = 0; i < _setup.numOfPosJoints; i++)
            {
                int length = _incomingReader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = _incomingReader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToPosIndex.Add(jointName, i);
                _setup.posIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfRotJoints = _incomingReader.ReadInt32();
            _setup.boneToRotIndex = new Dictionary<string, int>();
            _setup.rotIndexToBone = new Dictionary<int, string>();
            _data.rotations = new Vector3[_setup.numOfRotJoints];

            for (int i = 0; i < _setup.numOfRotJoints; i++)
            {
                int length = _incomingReader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = _incomingReader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToRotIndex.Add(jointName, i);
                _setup.rotIndexToBone.Add(i, jointName);
            }
            
            _setup.numOfMatrices = _incomingReader.ReadInt32();
            _setup.boneToMatIndex = new Dictionary<string, int>();
            _setup.matIndexToBone = new Dictionary<int, string>();
            _data.matrices = new Matrix4x4[_setup.numOfMatrices];

            for (int i = 0; i < _setup.numOfMatrices; i++)
            {
                int length = _incomingReader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = _incomingReader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToMatIndex.Add(jointName, i);
                _setup.matIndexToBone.Add(i, jointName);
            }
            
            _isFirstPayload = false;
        }
        
        for (int i = 0; i < _setup.numOfPosJoints; i++)
        {
            _data.positions[i] = _incomingReader.ReadVector3();
        }
        
        for (int i = 0; i < _setup.numOfRotJoints; i++)
        {
            _data.rotations[i] = _incomingReader.ReadVector3();
        }

        for (int i = 0; i < _setup.numOfMatrices; i++)
        {
            _data.matrices[i] = _incomingReader.ReadMatrix4x4();
        }
    }

    public enum OpCode : int
    {
        DoAction = 0,
        SetDirection = 1,
    }
    
    private int a = 0;
    private void DoActionTest()
    {
        WriteOpCode(OpCode.DoAction);
        if (a == 5) a = 0;
        _outgoingWriter.Write(a++);
        FlushOutgoingPacket();
    }
    
    private void SetDirection(Vector2 dir)
    {
        WriteOpCode(OpCode.SetDirection);
        _outgoingWriter.Write(dir);
        FlushOutgoingPacket();
    }

    private void WriteOpCode(OpCode op)
    {
        _outgoingWriter.Write((int) op);
    }

    private void FlushOutgoingPacket()
    {
        var data = _outgoingStream.ToArray();
        var lengthBytes = BitConverter.GetBytes(data.Length);
        Array.Reverse(lengthBytes);
        _networkStream.Write(lengthBytes);
        _networkStream.Write(data);
        _outgoingStream.SetLength(0);
    }
    
    private void Update()
    {
        if (_client.Connected && _incomingStream != null)
        {
            ProcessIncomingPackets();
            if (Input.GetKeyDown(KeyCode.Space)) DoActionTest();
            if (Input.GetKeyDown(KeyCode.Q)) SetDirection(Random.insideUnitCircle);
            if (Input.GetKeyDown(KeyCode.W)) SetDirection(Vector2.zero);
        }
    }

    private void ProcessIncomingPackets()
    {
        if (_incomingStream.Length < 4) return;
        lock (_incomingStream)
        {
            _incomingStream.Position = 0;
            var receivingBytesSize = _incomingReader.ReadInt32();
            var currentSize = _incomingStream.Length - _incomingStream.Position;
            if (currentSize < receivingBytesSize)
            {
                return;
            }

            ReadData();

            var discardCount = receivingBytesSize + 4 - _incomingStream.Position;
            for (int i = 0; i < discardCount; i++)
            {
                _incomingStream.ReadByte();
            }
            
            var remaining = _incomingStream.Read(_copyRemainBuffer);
            _incomingStream.SetLength(0);
            if (remaining != 0) 
                _incomingStream.Write(_copyRemainBuffer, 0, remaining);
        }

        ProcessData();
    }

    private void OnDestroy()
    {
        _client.Close();
    }

}
