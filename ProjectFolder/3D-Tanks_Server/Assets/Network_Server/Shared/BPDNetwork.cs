using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using System.Reflection;

//BIG PARSER DEMO NETWORK
//This is the base class for the Lidgren-based RPC framework of any multiplayer game we might need.
//This class should remain identical between the client and server builds, and should primarily act as the framework for reading and writing RPC parameters. Avoid putting server, client, or game-specific logic here as much as possible.
//WRITTEN BY LIAM SHELTON
public class BPDNetwork : MonoBehaviour
{
    public NetPeerConfiguration netConfig; //Clients and servers will need to work with this variable when initializing
    public NetPeer netPeer; //Please note that Lidgren's naming of this type can be something of a misnomer. The BigParser tank demo utilizes authoritative client-server architecture
    public bool DebugMessages;//Turn this off to disable BPDNetwork debug messages

    // Update is called once per frame
    protected virtual void Update()
    {
        NetLogger.DebugMessages = DebugMessages;
        ProcessMessages();
    }

    //This function should be called every frame, and will check for new UDP messages.
    //If they happen to be of the Data type, then the function will attempt to process them as an RPC call
    //This method will attempt to use inflection to invoke methods specified in RPC calls with included parameters.
    //It will check every script that shares the same GameObject as the one this script is attached to. Consider this when structuring management systems!
    protected void ProcessMessages()
    {
        List<NetIncomingMessage> incomingMessages = new List<NetIncomingMessage>();
        int numberOfMessages = netPeer.ReadMessages(incomingMessages);
        foreach (NetIncomingMessage message in incomingMessages)
        {
            long senderID = message.SenderConnection.RemoteUniqueIdentifier;
            NetLogger.Log($"Received message of type {message.MessageType.ToString()} from sender of id {senderID}");
            if (message.MessageType == NetIncomingMessageType.Data)
            {
                string functionName = message.ReadString();
                NetLogger.Log($"Recieved an RPC call for function {functionName} from sender of id {senderID}");
                Component[] myScripts = gameObject.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in myScripts)
                {
                    MethodInfo methodInfo = script.GetType().GetMethod(functionName);
                    if (methodInfo != null)
                    {
                        methodInfo.Invoke(script, ReadRPCParameters(message));
                    }
                }
            }
        }
    }


    //This function will attempt to convert the RPC data of an RPC call into an array of objects that can be used as parameters for invocation
    protected object[] ReadRPCParameters(NetIncomingMessage message)
    {
        List<object> parameters = new List<object>();

        parameters.Add(message.SenderConnection); //The first parameter of every RPC call should be internally set to the sender connection. This will allow the server and clients to ignore RPC calls from unauthorized sources, as well as make it easy to ignore unauthorized or invalid RPC calls

        string parametersDefinition = message.ReadString(); //The first string of every RPC will be a series of letters representing data types for parameters (eg IFB would mean the RPC contains an int, float, and a bool)

        //Parse out the RPC parameters in this loop. All possible data types and structures that need to be transmitted over the internet will have to exist in this else/if chain first.
        foreach (char character in parametersDefinition)
        {
            if (character == 'I') //Integer
            {
                var parameter = message.ReadInt32();
                parameters.Add(parameter);
            }
            else if (character == 'F') //Float
            {
                var parameter = message.ReadFloat();
                parameters.Add(parameter);
            }
            else if (character == 'S') //String
            {
                var parameter = message.ReadString();
                parameters.Add(parameter);
            }
            else if (character == 'B') //Boolean
            {
                var parameter = message.ReadBoolean();
                parameters.Add(parameter);
            }
            else if (character == 'V') //Vector3.  It is assumed that the data will be written as XYZ
            {
                var parameter = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
                parameters.Add(parameter);
            }
            else
            {
                NetLogger.LogError($"Unrecognized parameter of character definition {character}");
            }
        }
        return parameters.ToArray();
    }

    //When called from an inheriting script, this function can be used to write a set of parameter objects into a NetOutgoingMessage which Lidgren can trasmit as a UDP datagram
    protected void WriteRPCParameters(NetOutgoingMessage message, params object[] parameters)
    {
        string parametersDefinition = ""; //This will become the string that RPCs use to define their contents. For example, IFV means it should contain an integer, float, and vector3, in that order

        foreach (object obj in parameters) //Another foreach loop, this time to convert parameters into a string that the read function will use to determine the RPC contents
        {
            if (obj is int)
            {
                parametersDefinition = parametersDefinition + "I";
            }
            else if (obj is float)
            {
                parametersDefinition = parametersDefinition + "F";
            }
            else if (obj is string)
            {
                parametersDefinition = parametersDefinition + "S";
            }
            else if (obj is bool)
            {
                parametersDefinition = parametersDefinition + "B";
            }
            else if (obj is Vector3)
            {
                parametersDefinition = parametersDefinition + "V";
            }
            else
            {
                NetLogger.LogError($"Failed to write an RPC definition for object {obj}");
            }
        }
        message.Write(parametersDefinition);

        foreach (object obj in parameters) //This next foreach loop may look similar to the previous, but must be separate to ensure the definition section of the RPC is completely separate from the contents
        {
            if (obj is int)
            {
                int anInteger = (int)obj;
                message.Write(anInteger);
            }
            else if (obj is float)
            {
                float aFloat = (float)obj;
                message.Write(aFloat);
            }
            else if (obj is string)
            {
                string aString = (string)obj;
                message.Write(aString);
            }
            else if (obj is bool)
            {
                bool aBool = (bool)obj;
                message.Write(aBool);
            }
            else if (obj is Vector3)
            {
                Vector3 aVector = (Vector3)obj;
                message.Write(aVector.x);
                message.Write(aVector.y);
                message.Write(aVector.z);
            }
            else
            {
                NetLogger.LogError($"Failed to write RPC contents for object {obj}");
            }
        }
    }

    //protected void Log(string message)
    //{
    //    if (DebugMessages)
    //    {
    //        Debug.Log($"NETWORK: {message}");
    //    }
    //}
    //protected void LogWarning(string message)
    //{
    //    if (DebugMessages)
    //    {
    //        Debug.LogWarning($"NETWORK: {message}");
    //    }
    //}
    //protected void LogError(string message)
    //{
    //    if (DebugMessages)
    //    {
    //        Debug.LogError($"NETWORK: {message}");
    //    }
    //}
}

//NETLOGGER
//Use this to more easily enable or disable debug messages from the RPC framework 
public static class NetLogger
{
    public static bool DebugMessages;
    public static void Log(string message)
    {
        if (DebugMessages)
        {
            Debug.Log($"NETWORK: {message}");
        }
    }
    public static void LogWarning(string message)
    {
        if (DebugMessages)
        {
            Debug.LogWarning($"NETWORK: {message}");
        }
    }
    public static void LogError(string message)
    {
        if (DebugMessages)
        {
            Debug.LogError($"NETWORK: {message}");
        }
    }
}
