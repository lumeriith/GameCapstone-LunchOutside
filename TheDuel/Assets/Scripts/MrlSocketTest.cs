using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using Sirenix.OdinInspector;

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
    public Dictionary<int, string> boneNameByNodeIndex;


    private List<TestNode> _nodes;

    private TcpClient _client;

    private NetworkStream _networkStream;
    private byte[] _networkStreamBuffer;
    private MemoryStream _memStream;
    private BinaryReaderBE _memReader;
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
            if (!_client.Connected) return;
            _networkStream = _client.GetStream();
            _memStream = new MemoryStream();
            _memReader = new BinaryReaderBE(_memStream);
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

        Destroy(nodeTemplate);
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

        lock (_memStream)
        {
            _memStream.Position = _memStream.Length;
            _memStream.Write(_networkStreamBuffer, 0, bytesRead);
        }
        _networkStream.BeginRead(_networkStreamBuffer, 0, dataBufferSize, PutToMemStream, null);
    }

    struct ReceivedData
    {
        public Vector3[] positions;
        public Vector3[] rotations;
    }

    struct SetupData
    {
        public Dictionary<int, string> posIndexToBone;
        public Dictionary<int, string> rotIndexToBone;
        public Dictionary<string, int> boneToPosIndex;
        public Dictionary<string, int> boneToRotIndex;
        public int numOfPosJoints;
        public int numOfRotJoints;
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
                _nodes[i].transform.rotation = Quaternion.Euler(_data.rotations[rotIndex]);
                _nodes[i].MakeRed();
            }
        }
    }
    
    private void ReadData()
    {
        if (_isFirstPayload)
        {
            _setup.numOfPosJoints = _memReader.ReadInt32();
            _setup.numOfRotJoints = _memReader.ReadInt32();
            _setup.boneToPosIndex = new Dictionary<string, int>();
            _setup.boneToRotIndex = new Dictionary<string, int>();
            _setup.posIndexToBone = new Dictionary<int, string>();
            _setup.rotIndexToBone = new Dictionary<int, string>();

            char[] arr = new char[255];
            for (int i = 0; i < _setup.numOfPosJoints; i++)
            {
                int length = _memReader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = _memReader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToPosIndex.Add(jointName, i);
                _setup.posIndexToBone.Add(i, jointName);
            }

            for (int i = 0; i < _setup.numOfRotJoints; i++)
            {
                int length = _memReader.ReadInt32();
                for (int c = 0; c < length; c++)
                    arr[c] = _memReader.ReadChar();
                string jointName = new string(arr, 0, length);
                _setup.boneToRotIndex.Add(jointName, i);
                _setup.rotIndexToBone.Add(i, jointName);
            }

            _data.positions = new Vector3[_setup.numOfPosJoints];
            _data.rotations = new Vector3[_setup.numOfRotJoints];

            _isFirstPayload = false;
        }
        
        for (int i = 0; i < _setup.numOfPosJoints; i++)
        {
            _data.positions[i] = _memReader.ReadVector3();
        }
        
        for (int i = 0; i < _setup.numOfRotJoints; i++)
        {
            _data.rotations[i] = _memReader.ReadVector3();
        }
    }
    
    private void Update()
    {
        if (!_client.Connected || _memStream == null) return;

        if (_memStream.Length < 4) return;
        lock (_memStream)
        {
            _memStream.Position = 0;
            var receivingBytesSize = _memReader.ReadInt32();
            var currentSize = _memStream.Length - _memStream.Position;
            if (currentSize < receivingBytesSize)
            {
                return;
            }

            ReadData();
            
            var discardCount = receivingBytesSize + 4 - _memStream.Position;
            for (int i = 0; i < discardCount; i++)
            {
                _memStream.ReadByte();
            }
            
            var remaining = _memStream.Read(_copyRemainBuffer);
            _memStream.SetLength(0);
            if (remaining != 0) 
                _memStream.Write(_copyRemainBuffer, 0, remaining);
        }

        ProcessData();
    }

    private void OnDestroy()
    {
        _client.Close();
    }

}
