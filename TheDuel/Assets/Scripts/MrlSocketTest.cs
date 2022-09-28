using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;

public class MrlSocketTest : MonoBehaviour
{
    public int dataBufferSize = 1024 * 300;
    public string host = "127.0.0.1";
    public int port = 1369;
    public float visualizeScale = 0.02f;
    public GameObject nodeTemplate;
    public int nodeCount = 50;


    private List<GameObject> _nodes;

    private TcpClient _client;

    private NetworkStream _networkStream;
    private byte[] _networkStreamBuffer;
    private MemoryStream _memStream;
    private BinaryReaderBE _memReader;
    private byte[] _copyRemainBuffer;

    private void Start()
    {
        _nodes = new List<GameObject>();
        for (int i = 0; i < nodeCount; i++)
        {
            _nodes.Add(Instantiate(nodeTemplate));
        }
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

    private void PutToMemStream(IAsyncResult ar)
    {
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

    private void ReadData()
    {
        var nodeCount = _memReader.ReadInt32();
        for (int i = 0; i < nodeCount; i++)
        {
            _nodes[i].transform.SetPositionAndRotation(_memReader.ReadVector3() * visualizeScale,
                Quaternion.Euler(_memReader.ReadVector3()));
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
            if (remaining == 0) return;
            _memStream.Write(_copyRemainBuffer, 0, remaining);
        }
    }

    private void OnDestroy()
    {
        _client.Close();
    }

}
