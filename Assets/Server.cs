using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.Networking;

public class Server : MonoBehaviour {

    int maxConnections = 10;
    int port = 10543;
    int connectionId;
    int channelId;
    int hostId;

    void Start() {

        Debug.Log("Starting server...");

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        HostTopology topology = new HostTopology(config, 10);

        channelId = config.AddChannel(QosType.ReliableSequenced);
        NetworkServer.Configure(topology);
        hostId = NetworkTransport.AddHost(topology, port);
        Debug.Log("Host ID: " + hostId);
    }

    void Update() {
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] buffer = new byte[1024];
        int bufferSize = buffer.Length;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, buffer, bufferSize, out dataSize, out error);
        switch (recData) {
            case NetworkEventType.Nothing:
                break;
            case NetworkEventType.ConnectEvent:
                Debug.Log("Connected");
                break;
            case NetworkEventType.DataEvent:
                ProcessMessage(buffer);
                break;
            case NetworkEventType.DisconnectEvent:
                Debug.Log("Disconnected");
                break;
        }
        if(error != 0) Debug.Log("Error Receiving Message: " + (NetworkError)error);
    }

    public void ProcessMessage(byte[] buffer) {
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();

        NetworkObject nobj = formatter.Deserialize(stream) as NetworkObject;

        switch (nobj.networkId) {
            case 0:
                Debug.Log(((TextMessage)nobj).message);
                break;
        }
    }

    public void SendMessage(NetworkObject data) {
        byte[] buffer = new byte[1024];
        Stream stream = new MemoryStream(buffer);
        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, data);
        byte error;
        NetworkTransport.Send(hostId, connectionId, channelId, buffer, buffer.Length, out error);
        if (error != 0) Debug.Log("Send Message Error: " + (NetworkError)error);
    }
}
