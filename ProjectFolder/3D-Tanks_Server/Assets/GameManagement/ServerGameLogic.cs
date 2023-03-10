using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

[System.Serializable]
public class PlayerConnection
{
    public NetConnection connection;
    public int playerID;
    public string userName;

    public object inputData;
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
        theServer.InitializeServer();
    }
    // Start is called before the first frame update
    void Start()
    {
        //theServer.InitializeServer();
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

    public delegate void PlayerAdded(PlayerConnection player);
    public PlayerAdded playerAddedCallback;
    public static void AddPlayerConnection(NetConnection client, int ID, string username)
    {
        PlayerConnection newPlayer = new PlayerConnection();
        newPlayer.connection = client;
        newPlayer.playerID = ID;
        newPlayer.userName = username;
        instance.currentConnections.Add(newPlayer);
        instance.playerAddedCallback(newPlayer);
    }

    //WARNING: possibly painful lookups
    public static PlayerConnection GetPlayer(int ID)
    {
        NetLogger.Log($"GETPLAYER called for ID{ID}");
        foreach (PlayerConnection player in instance.currentConnections)
        {
            if (player.playerID == ID)
            {
                return player;
            }
        }
        return null;
    }
    public static PlayerConnection GetPlayer(NetConnection netConnection)
    {
        NetLogger.Log($"GETPLAYER called for connection ID {netConnection.RemoteUniqueIdentifier}");
        foreach (PlayerConnection player in instance.currentConnections)
        {
            if (player.connection == netConnection)
            {
                return player;
            }
        }
        return null;
    }

    //public void lsls(params)
    //{

    //}

    public void TestRPCForServer(NetConnection connection, string someTestMessage)
    {
        Debug.Log($"Client of ID {connection.RemoteUniqueIdentifier} has sent a message: {someTestMessage}");
    }
}
