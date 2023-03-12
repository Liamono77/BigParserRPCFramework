using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//public class PlayerInfo
//{
//    string 
//}

public class TankClientUniversal : MonoBehaviour
{
    public static TankClientUniversal instance;
    public ServerState serverState = ServerState.nullState;
    public enum ServerState
    {
        nullState,
        waitingForPlayers, //in this mode, the server will wait until enough players have joined and voted to start the game. Some servers may return to this mode after every match, or never at all
        preGame, //This is the state that usually happens when a server first starts the game. Think the opening cinematic from Destiny's Crucible, the countdown phase from Team Fortress 2, the countdown from Rocket League, etc. Players usually can spawn in instantly when the game is in this state
        playing, //This is the state for servers with active games. Players that join will usually be placed in respawn mode here
        postGame, //This is the state that typically happens after a team or player has won a match. Often portrays a cinematic shot of the "top three" players of the session
        leaderboard, //This is the state in which players view the leaderboard for the game that has just ended. This is typically what players will look at until the next round begins. Players may usually disconnect at this point without penalties.
    }

    public PlayerState playerState = PlayerState.Spawning;
    public enum PlayerState // Refer to this for client status updates when the server is running a game
    {
        Spawning, //Approve player respawn requests during this state
        Playing, //Allow player control of a tank during this state
        //Dying, //Perform death-specific processes during this state
    }

    [System.Serializable]
    public class PlayerConnectionInfo
    {
        //public int ID;
        public string username;
        public PlayerStatUpdate currentStats;
    }

    public List<PlayerConnectionInfo> connectedPlayers = new List<PlayerConnectionInfo>();

    public NetSync_Client myNetSync;

    private void Awake()
    {
        instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //void GetPlayer(string username)

    public delegate void ServerJoinCallback(string serverName, string gameMode, string description, string serverState);
    public ServerJoinCallback serverJoinCallback;
    //RPC from server when client has successfully connected
    public void ServerJoinResponse(NetConnection server, string serverName, string gameMode, string description, string serverState)
    {


        //ParseServerState(serverState);
        ServerStateUpdate(server, serverState);
        RefreshPlayerStats();
        serverJoinCallback(serverName, gameMode, description, serverState);
    }

    //RPC from the server to synchronize server state
    public void ServerStateUpdate(NetConnection server, string serverStateString)
    {
        ParseServerState(serverStateString);
    }

    void ParsePlayerState(string playerStateString)
    {
        if (PlayerState.TryParse(playerStateString, out playerState))
        {
            NetLogger.Log($"CLIENT HAS SUCCESSFULLY PARSED PLAYER STATE {playerState}");

        }
        else
        {
            NetLogger.LogError($"CLIENT HAS FAILED TO PARSE PLAYER STATE {playerStateString}");

        }
    }

    void ParseServerState(string serverStateString)
    {
       
        if (ServerState.TryParse(serverStateString, out serverState))
        {
            NetLogger.Log($"CLIENT HAS SUCCESSFULLY PARSED SERVER STATE {serverState}");
        }
        else
        {
            NetLogger.LogError($"CLIENT HAS FAILED TO PARSE SERVER STATE {serverStateString}");

        }
    }
    void RefreshPlayerStats()
    {
        connectedPlayers.Clear();
        BPDClient.instance.CallRPC("PlayerStatsRequest");
    }

    //RPC from the server to update a player's stats
    public void PlayerStatUpdate(NetConnection server, PlayerStatUpdate statUpdate)
    {
        bool hasFound = false;

        foreach (PlayerConnectionInfo player in connectedPlayers)
        {
            if (player.currentStats.playerID == statUpdate.playerID)
            {
                player.currentStats = statUpdate;
                hasFound = true;
                break;
            }
        }

        if (hasFound == false)
        {
            PlayerConnectionInfo playerConnectionInfo = new PlayerConnectionInfo();
            playerConnectionInfo.currentStats = statUpdate;
            playerConnectionInfo.username = statUpdate.username;
            connectedPlayers.Add(playerConnectionInfo);
        }
    }


    public delegate void RespawnCallback();
    public RespawnCallback respawnCallback;
    public void ServerRespawnNotice(NetConnection server, int netSyncID)
    {
        playerState = PlayerState.Playing;

        NetSync_Client theNetSync = SyncManager_Client.instance.GetNetSyncByID(netSyncID);
        if (theNetSync != null)
        {
            myNetSync = theNetSync;
        }
        else
        {
            NetLogger.LogError("RESPAWN: failed to locate my net sync object");
        }
    }

    public delegate void DeathCallback(float respawnTime, int killerSyncID, int killerPlayerID, string message);
    public DeathCallback deathCallback;
    public void ServerDeathNotice(NetConnection server, float respawnTime, int killerSyncID, int killerPlayerID, string message)
    {
        playerState = PlayerState.Spawning;
        deathCallback(respawnTime, killerSyncID, killerPlayerID, message);
    }


    public delegate void ServerChatCallback(string message);
    public ServerChatCallback serverChatCallback;
    public void ServerChatMessage(NetConnection server, string message)
    {

    }
}
