using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class BPDServer : BPDNetwork
{
    NetServer server;
    public int port = 603;


    //Try to initialize the server in Start() for convenience during development.
    void Start()
    {
        
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
        netConfig = new NetPeerConfiguration("BPDNetwork")
        {
            Port = port,
        };
        netPeer = new NetServer(netConfig);
        netPeer.Start();

        server = netPeer as NetServer;
    }

    //This overload of CallRPC will call an RPC to a single client, and will take a NetDeliveryMethod as a parameter
    //Remember the differences between UDP delivery methods. The two most important are:
    //Unreliable is best for 'blastable' data, such as transform data (position and rotation) or input data (bools and floats for player control inputs). This data will be sent out constantly, so it is OK if some packets don't arrive. The trade-off is dramatically higher speeds
    //Reliable is best for data that will only be sent occasionally and with guaranteed arrival. DO NOT send these on every frame! UDP packets with this will cause the sending application to wait for the destination to return a confirmation. It will repeatedly send the data if it doensn't recieve confirmation to guarantee arrival, at the expense of speed.
    public void CallRPC(string functionName, NetConnection recipient, NetDeliveryMethod deliveryMethod, params object[] parameters)
    {
        NetOutgoingMessage message = netPeer.CreateMessage();
        message.Write(functionName);
        WriteRPCParameters(message, parameters);
        server.SendMessage(message, recipient, deliveryMethod);
    }
    public void CallRPC(string theMessage, NetConnection recipient, params object[] parameters)
    {
        //CallR
    }
}
