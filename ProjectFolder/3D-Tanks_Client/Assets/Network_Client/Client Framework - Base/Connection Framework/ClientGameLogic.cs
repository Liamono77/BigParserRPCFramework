using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using UnityEngine.UI;

//CLIENT GAME LOGIC
//This script is primarily intended for logic that handles client-server connection management.
//Also responsible for disabling & enabling connection UI menus. 
//Misnomer: this script isn't meant for game-specific logic. Will probably rename to "ClientConnectionLogic"
//WRITTEN BY LIAM SHELTON
public class ClientGameLogic : MonoBehaviour
{
    public static ClientGameLogic instance; //Singleton reference

    public BPDClient theClient;

    public ClientGameState clientGameState = ClientGameState.connecting;

    public GameObject loginScreen;
    public GameObject connectScreen;

    public float handshakeTimer;
    public float handshakeFrequency = .5f;

    public float handshakeWaitDuration = 5f;
    public float handshakeWaitTimer;


    public float connectionTimer;
    public float connectionCheckFrequency = 5;
    public float timeoutWindow = 10f;

    //some quickndirty variables to test the rpc calling with
    public bool testButton1;
    public string testMessage1;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //theClient.InitializeClient(theClient.address, theClient.port);
    }

    // Update is called once per frame
    void Update()
    {
        if (testButton1)
        {
            testButton1 = false;
            theClient.CallRPC("TestRPCForServer", Lidgren.Network.NetDeliveryMethod.ReliableOrdered, testMessage1);
        }

        if (clientGameState == ClientGameState.waitingforHandshake)
        {
            waitForHandshake();
        }

        loginScreen.SetActive(clientGameState == ClientGameState.login);
        connectScreen.SetActive(clientGameState == ClientGameState.connecting);
    }

    public void MonitorConnectionStatus()
    {

    }

    public void waitForHandshake()
    {
        if (Time.time > handshakeTimer)
        {
            handshakeTimer = Time.time + handshakeFrequency;
            theClient.CallRPC("Handshake");
        }

        handshakeWaitTimer = handshakeWaitTimer - Time.deltaTime;
        if (handshakeWaitTimer <= 0)
        {
            clientGameState = ClientGameState.connecting;
        }
    }

    public void ConnectToServer(string address, int port)
    {
        theClient.InitializeClient(address, port);
        //theClient.CallRPC("Handshake");
        clientGameState = ClientGameState.waitingforHandshake;
        handshakeWaitTimer = handshakeWaitDuration;
    }
    public void HandshakeResponse(NetConnection server, int responseCode)
    {
        //use the response code to distinguish between matchmaker connections and game server connections
        if (responseCode == 0)
        {
            clientGameState = ClientGameState.login;
        }
    }

    //Call this from other scripts to switch the game state to lobby
    public void SwitchToLobby()
    {
        clientGameState = ClientGameState.lobby;
    }

    public void TestRPCForClient(NetConnection serverConnection, string aMessage)
    {
        Debug.Log($"The server has sent this client a test rpc with message {aMessage}");
    }
}

public enum ClientGameState
{
    connecting,
    waitingforHandshake,
    login,
    lobby,
}
