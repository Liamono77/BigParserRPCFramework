using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

[System.Serializable]
public class PlayerConnection
{
    public NetConnection connection;
    public string userName;
}

public class ServerGameLogic : MonoBehaviour
{
    public static ServerGameLogic instance; //Singleton Reference

    public BPDServer theServer;

    public List<PlayerConnection> currentConnections = new List<PlayerConnection>();



    //public float connectionCheckFrequency


    //some quickndirty variables to test the rpc calling with
    public bool testButton1;
    public string testMessage1;

    public bool testButton2;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        theServer.InitializeServer();
    }

    // Update is called once per frame
    void Update()
    {
        if (testButton1)
        {
            testButton1 = false;
            theServer.CallRPC("TestRPCForClient", testMessage1);
        }
        if (testButton2)
        {
            testButton2 = false;
            theServer.CallRPC("TestLoginRPC");
        }
    }

    public void ClientConnectionCheck(NetConnection sender)
    {
        theServer.CallRPC("SeverConnectionReply", sender);
    }

    //RPC from clients to help them identify if they've reached a server
    public void Handshake(NetConnection sender)
    {
        theServer.CallRPC("HandshakeResponse", sender, NetDeliveryMethod.ReliableOrdered, 0);
    }

    public void TestRPCForServer(NetConnection connection, string someTestMessage)
    {
        Debug.Log($"Client of ID {connection.RemoteUniqueIdentifier} has sent a message: {someTestMessage}");
    }
}
