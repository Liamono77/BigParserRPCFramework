using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class ServerGameLogic : MonoBehaviour
{
    public static ServerGameLogic instance; //Singleton Reference

    public BPDServer theServer;

    //some quickndirty variables to test the rpc calling with
    public bool testButton1;
    public string testMessage1;

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
            //theServer.CallRPC("TestRPCForClient", theServer.ser)
        }
    }

    public void TestRPCForServer(NetConnection connection, string someTestMessage)
    {
        Debug.Log($"Client of ID {connection.RemoteUniqueIdentifier} has sent a message: {someTestMessage}");
    }
}
