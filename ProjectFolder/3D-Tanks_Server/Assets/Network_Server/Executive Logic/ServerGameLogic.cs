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

    public object bigparserData; //Use this for persistent player data that should be pulled from BigParser for a specific game (equipment and cosmetic unlocks, currencies, lifetime performance statistics, etc)
    public object inputData; //Use this for player input data structures that all servers for a specific game are designed to listen for.
    public object serverData; //Use this for non-persistent game server-specific data that will be utilized by all servers of a particular game in each session (universal leaderboard data (kills & deaths, objective score), current loadout, current team, possibly playable entity types) 
    public object specialData; //Use this for non-persistent gamemode-specific data structures (eg goomba stomps, owned AI-controlled units, remaining lives,  current powerups, scrap/gold/fuel,  etc.)
}

public class ServerGameLogic : MonoBehaviour
{
    public static ServerGameLogic instance; //Singleton Reference

    public BPDServer theServer;

    public List<PlayerConnection> currentConnections = new List<PlayerConnection>();
    public int lastID = 0;


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
    public PlayerAdded playerAddedCallback; //use this callback in other scripts for functionality based upon new players joining
    public static void AddPlayerConnection(NetConnection client, string reason, string username)
    {
        NetLogger.Log($"CONNECTION: JOIN: Player of username '{username}' (client ID: '{client.RemoteUniqueIdentifier}') has been approved for full player connection. Reason: {reason}");
        PlayerConnection newPlayer = new PlayerConnection();
        newPlayer.connection = client;

        //give the player a unique ID for this server
        newPlayer.playerID = instance.lastID;
        instance.lastID++;

        newPlayer.userName = username;
        instance.currentConnections.Add(newPlayer);
        instance.playerAddedCallback(newPlayer);
    }

    public delegate void PlayerDisconnected(PlayerConnection player);
    public PlayerDisconnected playerDisconnectedCallback; //use this callback in other scripts for functionality based upon players leaving or disconnecting
    public static void RemovePlayerConnection(PlayerConnection player, string reason)
    {
        NetLogger.Log($"CONNECTION: LEAVE: Player of username '{player.userName}' (client ID: '{player.connection.RemoteUniqueIdentifier}') has been disconnected. Reason: {reason}");
        instance.currentConnections.Remove(player);
        instance.playerDisconnectedCallback(player);
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
