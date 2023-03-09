using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class BPDClient : BPDNetwork
{
    NetClient client;

    //Set the address to the IP address of the server, and the port to whatever port the server is hosted on. If attempting to connect via LAN, the IP address can be set to 127.0.0.1 for ease of access.
    //If attempting to connect across multiple networks, then you will need to set the address to the PUBLIC  IP address of the server, provided the server is on an open port (use portforwarding to make this work). Use http://www.ipmonkey.com/ in the server's local area network to figure out what the clients need to enter 

    public string address = "127.0.0.1";
    public int port = 603;

    void Start()
    {
        //InitializeClient(address, port);
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    //Calling this function will make the client attempt to connect to the server specified 
    public void InitializeClient(string address, int port)
    {
        netConfig = new NetPeerConfiguration("BPDNetwork"); //Make sure this string matches whatever is specified by the server, as Lidgren uses this like a security check.
        netPeer = new NetClient(netConfig);
        netPeer.Start();
        netPeer.Connect(host: address, port: port);
        client = netPeer as NetClient;
    }

    //This overload of CallRPC will send an RPC to the server, and will take a NetDeliveryMethod as a parameter
    //Remember the differences between UDP delivery methods. The two most important are:
    //Unreliable is best for 'blastable' data, such as transform data (position and rotation) or input data (bools and floats for player control inputs). This data will be sent out constantly, so it is OK if some packets don't arrive. The trade-off is dramatically higher speeds
    //Reliable is best for data that will only be sent occasionally and with guaranteed arrival. DO NOT send these on every frame! UDP packets with this will cause the sending application to wait for the destination to return a confirmation. It will repeatedly send the data if it doensn't recieve confirmation to guarantee arrival, at the expense of speed.
    public void CallRPC(string functionName, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        client.SendMessage(message, deliveryMethod);
        Debug.Log($"Sent an RPC call to server for function {functionName}");
    }
}
