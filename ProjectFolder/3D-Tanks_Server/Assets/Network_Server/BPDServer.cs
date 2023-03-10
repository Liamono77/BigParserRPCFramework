using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//BIG PARSER DEMO SERVER
//This derivation of BPDNetwork contains server-specific RPC functionality.
//It is meant to be reusable between game projects, so avoid putting game-specific functionality here if possible. Implements such logic in other scripts, get a reference to this script, and call the public RPC methods from this script.
//WRITTEN BY LIAM SHELTON
public class BPDServer : BPDNetwork
{
    NetServer server; //Lidgren defaults to peer-to-peer functionality, so one must specify that an implementation is server or client when client-server architecture is desired.

    //Use 'port' to specify which port you want to host the server on. 
    //Note that no IP address specification is necessary for the BPDServer. This is because the server, acting as the host, only has one possible IP address that it can host on (specifically the public adress of the WIFI or Ethernet connection of the computer running the server application).
    public int port = 603;

    public static BPDServer instance;//Singleton Reference (mostly for net sync systems)


    private void Awake()
    {
        instance = this;
    }

    //run base update function to listen for RPCs from clients. If additional update functionality is desired (possibly for BigParser-specific listeners), it can be set up here.
    protected override void Update()
    {
        base.Update();
    }

    //Call this function to make the server actually start. It will attempt to hook up with the UDP socket at 'port', and will then listen for incoming messages.
    //If an error message pops up in the Debug console complaining about socket exceptions, then chances are the socket is either currently being used by another application or was recently occupied by an application. You can almost always fix this by setting the port to something one more or one less than what it current is.
    public void InitializeServer()
    {
        netConfig = new NetPeerConfiguration("BPDNetwork") //Make sure this string matches whatever is specified by the server, as Lidgren uses this like a security check.
        {
            Port = port,
        };
        netPeer = new NetServer(netConfig);
        netPeer.Start();

        server = netPeer as NetServer;
    }

    //Remember the differences between UDP delivery methods. The two most important are:
    //Unreliable is best for 'blastable' data, such as transform data (position and rotation) or input data (bools and floats for player control inputs). This data will be sent out constantly, so it is OK if some packets don't arrive. The trade-off is dramatically higher speeds
    //ReliableOrdered is best for data that will only be sent occasionally, with guaranteed arrival, and with ordering. DO NOT send these on every frame! UDP packets with this will cause the sending application to wait for the destination to return a confirmation. It will repeatedly send the data if it doensn't recieve confirmation to guarantee arrival, at the expense of speed.

    //Calling this method will send an RPC to a single client, and will take a NetDeliveryMethod as a parameter
    public void CallRPC(string functionName, NetConnection recipient, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        server.SendMessage(message, recipient, deliveryMethod);
        NetLogger.Log($"Sent an RPC call to client of ID {recipient.RemoteUniqueIdentifier} for function {functionName}");
    }
    //This overload of CallRPC will send an RPC to a single client with the delivery method of reliable ordered.
    public void CallRPC(string functionName, NetConnection recipient, params object[] parameters)
    {
        CallRPC(functionName, recipient, NetDeliveryMethod.ReliableOrdered, parameters);
    }

    //This overload will send an RPC to all clients with the specified NetDeliveryMethod
    public void CallRPC(string functionName, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        server.SendToAll(message, deliveryMethod);
        NetLogger.Log($"Sent an RPC call to all clients for function {functionName}");
    }

    //This overload will send an RPC to all clients with the delivery method of ReliableOrdered
    public void CallRPC(string functionName, params object[] parameters)
    {
        CallRPC(functionName, NetDeliveryMethod.ReliableOrdered, parameters);
    }
}
