﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class BPDServer : BPDNetwork
{
    NetServer server;

    //Use 'port' to specify which port you want to host the server on. 
    //Note that no IP address specification is necessary for the BPDServer. This is because the server, acting as the host, only has one possible IP address that it can host on (specifically the public adress of the WIFI or Ethernet connection of the computer running the server application).
    public int port = 603;


    //Try to initialize the server in Start() for convenience during development.
    void Start()
    {
    //    InitializeServer();
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

    //This overload of CallRPC will send an RPC to a single client, and will take a NetDeliveryMethod as a parameter
    //Remember the differences between UDP delivery methods. The two most important are:
    //Unreliable is best for 'blastable' data, such as transform data (position and rotation) or input data (bools and floats for player control inputs). This data will be sent out constantly, so it is OK if some packets don't arrive. The trade-off is dramatically higher speeds
    //ReliableOrdered is best for data that will only be sent occasionally and with guaranteed arrival. DO NOT send these on every frame! UDP packets with this will cause the sending application to wait for the destination to return a confirmation. It will repeatedly send the data if it doensn't recieve confirmation to guarantee arrival, at the expense of speed.
    public void CallRPC(string functionName, NetConnection recipient, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        server.SendMessage(message, recipient, deliveryMethod);
        Debug.Log($"Sent an RPC call to client of ID {recipient.RemoteUniqueIdentifier} for function {functionName}");
    }
    public void CallRPC(string functionName, NetConnection recipient, params object[] parameters)
    {
        CallRPC(functionName, recipient, NetDeliveryMethod.ReliableOrdered, parameters);
    }


    public void CallRPC(string functionName, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        server.SendToAll(message, deliveryMethod);
        Debug.Log($"Sent an RPC call to all clients for function {functionName}");
    }

    public void CallRPC(string functionName, params object[] parameters)
    {
        CallRPC(functionName, NetDeliveryMethod.ReliableOrdered, parameters);
    }
}
