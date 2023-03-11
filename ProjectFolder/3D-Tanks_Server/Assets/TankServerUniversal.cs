using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//TANK SERVER PLAYER DATA
//Define non-persistent player data that will be shared between 3D-Tanks servers here
//This data should not be deleted during a game session unless a player disconnects. Make it persist between spawn cycles
[System.Serializable]
public class TankServerPlayerData
{
    public TankServerUniversal tankServerRef;

    public PlayerConnection myConnection;

    public bool isReady; //Has the player voted to start a game?

    public PlayerState playerState = PlayerState.Spawning;
    public enum PlayerState // Refer to this for client status updates when the server is running a game
    {
        Spawning, //Approve player respawn requests during this state
        Playing, //Allow player control of a tank during this state
        Dying, //Perform death-specific processes during this state
    }

    public float deathTimer;
    public float deathDuration = 3f;

    public GameObject currentTank;

    //public void SendPlayerStatusUpdate()
    //{
    //    BPDServer.instance.CallRPC("ClientStatusUpdate", myConnection, tankServerRef.gameModeType.ToString(), )
    //}
}

//TANK SERVER UNIVERSAL
//Use this as a modular unit for 3D-Tanks servers.
//Like other online multiplayer games, all servers will have things like pre and post-rounds, logic for linking player tanks to player inputs, logic for players joining active games, etc.
//Set that stuff up here, so that devs can set up gamemode-specific server logic in separate scripts.
//WRITTEN BY LIAM SHELTON
public class TankServerUniversal : MonoBehaviour
{
    public string serverName; //what will this sever be displayed as to players? Send it to BigParser!
    public string gameModeType; //what game mode will this server have (team deathmatch, capture the flag, domination, rumble, etc)? Try to match the naming conventions of other servers sharing the same mode.
    public string serverDescription; //give this server a description for players to view when first connecting.

    public List<TankServerPlayerData> tankServerDatas = new List<TankServerPlayerData>();

    public ServerState serverState = ServerState.waitingForPlayers;
    public enum ServerState
    {
        waitingForPlayers, //in this mode, the server will wait until enough players have joined and voted to start the game. Some servers may return to this mode after every match, or never at all
        preGame, //This is the state that usually happens when a server first starts the game. Think the opening cinematic from Destiny's Crucible, the countdown phase from Team Fortress 2, the countdown from Rocket League, etc. Players usually can spawn in instantly when the game is in this state
        playing, //This is the state for servers with active games. Players that join will usually be placed in respawn mode here
        postGame, //This is the state that typically happens after a team or player has won a match. Often portrays a cinematic shot of the "top three" players of the session
        leaderboard, //This is the state in which players view the leaderboard for the game that has just ended. This is typically what players will look at until the next round begins. Players may usually disconnect at this point without penalties.
    }

    public int minVotesToStart = 1;

    public GameObject tankPrefab;


    // Start is called before the first frame update
    void Start()
    {
        ServerGameLogic.instance.playerAddedCallback += OnPlayerConnection;
        ServerGameLogic.instance.playerDisconnectedCallback += OnPlayerDisconnect;

    }

    // Update is called once per frame
    void Update()
    {
        if (serverState == ServerState.waitingForPlayers)
        {
            Update_WaitingForPlayers();
        }
    }

    //Monitor the number of players voting to start a game.
    void Update_WaitingForPlayers()
    {
        int numberOfVotes = 0;
        foreach (PlayerConnection player in ServerGameLogic.instance.currentConnections)
        {
            if ((player.serverData as TankServerPlayerData).isReady)
            {
                numberOfVotes++;
            }
        }
        if (numberOfVotes >= minVotesToStart)
        {
            NetLogger.Log("START VOTE HAS PASSED");
            SwitchToPreGame();
        }

    }

    void SwitchToPreGame()
    {
        serverState = ServerState.preGame;
        BPDServer.instance.CallRPC("SwitchToPreGame");
    }


    public void OnPlayerConnection(PlayerConnection player)
    {
        TankServerPlayerData playerData = new TankServerPlayerData();
        playerData.myConnection = player;
        playerData.tankServerRef = this;
        player.serverData = playerData;
        tankServerDatas.Add(playerData);
        SendServerInfo(player);
    }

    public void OnPlayerDisconnect(PlayerConnection player)
    {
        TankServerPlayerData playerData = (player.serverData as TankServerPlayerData);
        tankServerDatas.Remove(playerData);
        //SendServerInfo(player);
    }

    public void SendServerInfo(PlayerConnection player)
    {
        BPDServer.instance.CallRPC("ServerJoinResponse", player.connection, serverName, gameModeType, serverDescription, serverState.ToString());
    }

    public void SpawnTank(PlayerConnection player)
    {
        (player.serverData as TankServerPlayerData).currentTank = GameObject.Instantiate(tankPrefab);
    }


    //CLIENT RPCs BEYOND THIS POINT

    //Request from player to spawn
    public delegate void OnPlayerSpawn(PlayerConnection player);
    public OnPlayerSpawn onPlayerSpawn; //Use this callback in other scripts to extend player spawning behaviors
    public void PlayerSpawnRequest(NetConnection sender)
    {
        PlayerConnection player = ServerGameLogic.GetPlayer(sender);

        if (player != null)
        {
            NetLogger.Log($"SPAWN: Attempting to spawn a tank for player {player.userName}");
            SpawnTank(player);
            onPlayerSpawn(player);
        }
        else
        {
            NetLogger.LogWarning($"SPAWN: Failed to spawn a tank for player of ID {sender.RemoteUniqueIdentifier}");
        }
    }

    //This RPC represents a request from a client to change one thing about current tank loadout. Be sure to verify this request using BigParser data!
    public void PlayerLoadoutChange(NetConnection client)
    {

    }
}
