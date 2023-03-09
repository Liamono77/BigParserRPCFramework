using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class ClientGameLogic : MonoBehaviour
{
    public static ClientGameLogic instance; //Singleton reference

    public BPDClient theClient;


    //some quickndirty variables to test the rpc calling with
    public bool testButton1;
    public string testMessage1;



    // Start is called before the first frame update
    void Start()
    {
        theClient.InitializeClient(theClient.address, theClient.port);
    }

    // Update is called once per frame
    void Update()
    {
        if (testButton1)
        {
            testButton1 = false;
            theClient.CallRPC("TestRPCForServer", Lidgren.Network.NetDeliveryMethod.ReliableOrdered, testMessage1);
        }
    }

    public void TestRPCForClient(NetConnection serverConnection, string aMessage)
    {
        Debug.Log($"The server has sent this client a test rpc with message {aMessage}");
    }
}
