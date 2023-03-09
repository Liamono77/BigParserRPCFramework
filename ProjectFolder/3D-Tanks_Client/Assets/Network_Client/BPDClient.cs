using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//BIG PARSER DEMO CLIENT
//This is derivative of the BPDNetwork script containing the adjustments necessary for the client side.
//It is meant to be reusable between game projects, so avoid putting game-specific functionality here if possible. Implements such logic in other scripts, get a reference to this script, and call the public RPC methods from this script.
//RPC calls from clients do not ever need to specify a reciever, because the only destination that will listen should be the server.
//WRITTEN BY LIAM SHELTON
public class BPDClient : BPDNetwork
{
    NetClient client; //Lidgren defaults to peer-to-peer functionality, so one must specify that an implementation is server or client when client-server architecture is desired.

    //Set the address to the IP address of the server, and the port to whatever port the server is hosted on. If attempting to connect via LAN, the IP address can be set to 127.0.0.1 for ease of access.
    //If attempting to connect across multiple networks, then you will need to set the address to the PUBLIC  IP address of the server, provided the server is on an open port (use portforwarding to make this work). Use http://www.ipmonkey.com/ in the server's local area network to figure out what the clients need to enter 

    public string address = "127.0.0.1";
    public int port = 603;


    //Run base update function to listen for RPC calls
    protected override void Update()
    {
        base.Update();
    }

    //Calling this function will make the client attempt to connect to the server specified
    //If nothing happens after calling this, then chances are it wasn't able to connect to the server at the specified address and port. Check to make sure the server is actually online!
    public void InitializeClient(string address, int port)
    {
        netConfig = new NetPeerConfiguration("BPDNetwork"); //Make sure this string matches whatever is specified by the server, as Lidgren uses this like a security check.
        netPeer = new NetClient(netConfig);
        netPeer.Start();
        netPeer.Connect(host: address, port: port);
        client = netPeer as NetClient;
    }

    //Remember the differences between UDP delivery methods. The two most important are:
    //Unreliable is best for 'blastable' data, such as transform data (position and rotation) or input data (bools and floats for player control inputs). This data will be sent out constantly, so it is OK if some packets don't arrive. The trade-off is dramatically higher speeds
    //ReliableOrdered is best for data that will only be sent occasionally, with guaranteed arrival, and with ordering. DO NOT send these on every frame! UDP packets with this will cause the sending application to wait for the destination to return a confirmation. It will repeatedly send the data if it doensn't recieve confirmation to guarantee arrival, at the expense of speed.

    //This will send an RPC to the server, and will take a NetDeliveryMethod as a parameter
    public void CallRPC(string functionName, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        client.SendMessage(message, deliveryMethod);
        NetLogger.Log($"Sent an RPC call to server for function {functionName}");
    }

    //This overload will send an RPC to the server with reliable ordered delivery.
    public void CallRPC( string functionName, params object[] parameters)
    {
        CallRPC(functionName, NetDeliveryMethod.ReliableOrdered, parameters);
    }
}
