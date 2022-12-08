using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Net.Sockets;
using Sirenix.OdinInspector;
using Random = UnityEngine.Random;

public class ModelConnection : SerializedMonoBehaviour
{
    public bool isConnected => _client != null && _client.Connected;
    public BinaryWriterBE writer => _outgoingWriter;

    public Action onConnected;
    
    [NonSerialized]
    public Action<BinaryReaderBE> dataReader;

    
    public int dataBufferSize = 1024 * 300;
    public string host = "127.0.0.1";
    public int port = 1369;
    
    
    private TcpClient _client;
    private NetworkStream _networkStream;
    private byte[] _networkStreamBuffer;
    private MemoryStream _incomingStream;
    private MemoryStream _outgoingStream;
    private BinaryReaderBE _incomingReader;
    private BinaryWriterBE _outgoingWriter;
    private byte[] _copyRemainBuffer;
    
    private void Start()
    {
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
            onConnected?.Invoke();
        }, _client);
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
    
    private void Update()
    {
        if (!isConnected) return;
        
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

            dataReader.Invoke(_incomingReader);

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
    }
    
    public void FlushOutgoingPacket()
    {
        var data = _outgoingStream.ToArray();
        var lengthBytes = BitConverter.GetBytes(data.Length);
        Array.Reverse(lengthBytes);
        _networkStream.Write(lengthBytes);
        _networkStream.Write(data);
        _outgoingStream.SetLength(0);
    }

    private void OnDestroy()
    {
        _client.Close();
    }
}
