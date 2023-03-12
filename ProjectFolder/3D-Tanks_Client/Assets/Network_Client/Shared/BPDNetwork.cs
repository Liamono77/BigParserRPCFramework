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
    public List<GameObject> externalReferences = new List<GameObject>(); //This is a list of additional GameObjects to search if the script fails to find an RPC's destination on the gameObject its attached to.

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

                bool localAttempt = AttemptInvocation(gameObject, functionName, message);
                if (localAttempt == false)
                {
                    bool externalAttempt = false;
                    foreach (GameObject reference in externalReferences)
                    {
                        if (AttemptInvocation(reference, functionName, message) == true)
                        {
                            externalAttempt = true;
                            break;
                        }
                    }
                    if (externalAttempt == false)
                    {
                        //Log this as a normal error to guarantee it shows up. If you see this error, then something has probably gone horribly wrong
                        Debug.LogError($"CRITICAL: Failed to find an invocation target for an RPC of name {functionName} from sender of ID {senderID}");
                    }

                }
            }
        }
    }

    //This is an encapsulation of logic that was formerly contained in the ProcessMessages() method. It can be used to attempt an RPC-based invocation on a game object.
    bool AttemptInvocation(GameObject gameObject, string functionName, NetIncomingMessage message)
    {
        bool hasFoundScript = false;
        Component[] myScripts = gameObject.GetComponents<MonoBehaviour>();
        foreach (MonoBehaviour script in myScripts)
        {
            MethodInfo methodInfo = script.GetType().GetMethod(functionName);
            if (methodInfo != null)
            {
                methodInfo.Invoke(script, ReadRPCParametersNew(message));
                hasFoundScript = true;
            }
        }

        return hasFoundScript;
    }

    //This function will attempt to convert the RPC data of an RPC call into an array of objects that can be used as parameters for invocation
    protected object[] ReadRPCParametersNew(NetIncomingMessage message)
    {
        string parametersDefinition = message.ReadString(); //The first string of every RPC will be a series of letters representing data types for parameters (eg IFB would mean the RPC contains an int, float, and a bool)
        char[] parametersAsArray = parametersDefinition.ToCharArray(); //convert to char array so that we can modify the contents
        List<object> parameters = ReadParametersAsList(ref message, ref parametersAsArray); //run the list-builder function on the char array
        parameters.Insert(0, message.SenderConnection);//Make sure to put the sender at the front.

        return parameters.ToArray();
    }

    //An encapsulation of the ReadRPCParameters logic that has the capability of calling itself.
    List<object> ReadParametersAsList(ref NetIncomingMessage message, ref char[] parametersDefinition)
    {
        List<object> parameters = new List<object>();
        //Parse out the RPC parameters in this loop. All possible data types and structures that need to be transmitted over the internet will have to exist in this else/if chain first.
        for (int i = 0; i < parametersDefinition.Length; i++)
        {
            //tempString
            char character = parametersDefinition[i];

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
            else if (character == 'T')
            {
                Vector3 pos = new Vector3(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
                Quaternion rot = Quaternion.Euler(message.ReadFloat(), message.ReadFloat(), message.ReadFloat());
                TransformInfo newTransformInfo = new TransformInfo(pos, rot);
                parameters.Add(newTransformInfo);
            }
            else if (character == 'P') //player stat update
            {
                PlayerStatUpdate playerStatUpdate = new PlayerStatUpdate();
                playerStatUpdate.playerID = message.ReadInt32();
                playerStatUpdate.username = message.ReadString();
                int statCount = message.ReadInt32();
                for (int o = 0; o < statCount; o++)
                {
                    StatObject statObject = new StatObject(message.ReadString(), message.ReadInt32());
                    playerStatUpdate.statObjects.Add(statObject);
                }
                //playerStatUpdate.statCount = statCount;

                parameters.Add(playerStatUpdate);
            }

            //WARNING: Sensitive logic beyond this point. 
            //Possible performance drain here. Debugger suggests that RPCs with empty parameter bundles still cause looping to occur through entire transmission. 
            else if (character == 'A') //Characters 'A' designate the start of a bundled object array. 
            {
                NetLogger.LogWarning($"CALLING DEF READER RECURSIVELY ON ARRAY DEFINITION AT INDEX {i}");

                //mark the current character so that recursion doesnt see it
                parametersDefinition[i] = '_';

                List<object> internalList = ReadParametersAsList(ref message, ref parametersDefinition);
                parameters.Add(internalList.ToArray());


            }
            else if (character == 'a') //Characters 'a' designate the end of a bundled object array.
            {
                //stop the loop here!
                parametersDefinition[i] = '_';
                return parameters;
            }
            else if (character == '_') //Marks a character that has been read once and should be ignored from now on
            {
                //NetLogger.LogWarning("skipped a param that has probably been read already");
            }
            else
            {
                NetLogger.LogError($"Unrecognized parameter of character definition {character}");
            }

            //After finishing with the character, mark it as read.
            parametersDefinition[i] = '_';
        }

        return parameters;
    }

    //When called from an inheriting script, this function can be used to write a set of parameter objects into a NetOutgoingMessage which Lidgren can trasmit as a UDP datagram
    protected void WriteRPCParameters(NetOutgoingMessage message, params object[] parameters)
    {
        string parametersDefinition = ""; //This will become the string that RPCs use to define their contents. For example, IFV means it should contain an integer, float, and vector3, in that order

        WriteParameterDef(ref parametersDefinition, parameters);
        Debug.LogWarning($"Parameter definition written as {parametersDefinition}");
        message.Write(parametersDefinition);

        WriteParameterContents(ref message, parameters);
    }


    void WriteParameterDef(ref string parametersDefinition, params object[] parameters)
    {
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
            else if (obj is TransformInfo)
            {
                parametersDefinition = parametersDefinition + "T";
            }
            else if (obj is PlayerStatUpdate)
            {
                parametersDefinition = parametersDefinition + "P";
            }
            else if (obj is object[])
            {
                //PREVIOUS
                //Debug.LogWarning("CALLING DEF WRITER RECURSIVELY ON OBJECT ARRAY");
                //WriteParameterDef(ref parametersDefinition, obj as object[]);

                //NEW
                object[] objectArray = obj as object[];
                int arrayLength = objectArray.Length;
                parametersDefinition = parametersDefinition + $"A";


                Debug.LogWarning($"CALLING DEF WRITER RECURSIVELY ON OBJECT ARRAY OF LENGTH {arrayLength}");
                WriteParameterDef(ref parametersDefinition, obj as object[]);
                parametersDefinition = parametersDefinition + $"a";
            }
            else
            {
                NetLogger.LogError($"Failed to write an RPC definition for object {obj}");
            }
        }
    }

    void WriteParameterContents(ref NetOutgoingMessage message, params object[] parameters)
    {
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
            else if (obj is TransformInfo)
            {
                TransformInfo theInfo = (TransformInfo)obj;
                message.Write(theInfo.position.x);
                message.Write(theInfo.position.y);
                message.Write(theInfo.position.z);
                Vector3 euler = theInfo.rotation.eulerAngles;
                message.Write(euler.x);
                message.Write(euler.y);
                message.Write(euler.z);

            }
            else if (obj is PlayerStatUpdate)
            {
                PlayerStatUpdate theUpdate = (PlayerStatUpdate)obj;
                message.Write(theUpdate.playerID);
                message.Write(theUpdate.username);
                message.Write(theUpdate.statObjects.Count);
                foreach (StatObject statObject in theUpdate.statObjects)
                {
                    message.Write(statObject.name);
                    message.Write(statObject.statValue);
                }
            }
            else if (obj is object[])
            {
                Debug.LogWarning("CALLING CONTENT WRITER RECURSIVELY ON OBJECT ARRAY");
                WriteParameterContents(ref message, obj as object[]);
            }
            else
            {
                NetLogger.LogError($"Failed to write RPC contents for object {obj}");
            }
        }
    }
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

public class TransformInfo
{
    public TransformInfo()
    {

    }
    public TransformInfo(Transform transform)
    {
        position = transform.position;
        rotation = transform.rotation;
    }
    public TransformInfo(Vector3 pos, Quaternion rot)
    {
        position = pos;
        rotation = rot;
    }
    public Vector3 position;
    public Quaternion rotation;
}

[System.Serializable]
public class PlayerStatUpdate
{
    public int playerID;
    public string username;
    //public int statCount;
    public List<StatObject> statObjects = new List<StatObject>();
}

[System.Serializable]
public class StatObject
{
    public StatObject()
    {

    }
    public StatObject(string sname, int value)
    {
        name = sname;
        statValue = value;
    }
    public string name;
    public int statValue;
}
