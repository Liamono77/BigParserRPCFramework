using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//SERVER LOGIN MANAGER
//This is a script that should handle player login requests. 
//This script is probably where methods for sending REST requests to BigParser will be triggered.
//WRITTEN BY LIAM SHELTON
public class ServerLoginManager : MonoBehaviour
{
    //The majority of this section will be swapped out for BigParser-integrated functionality
    [System.Serializable]
    public class PlayerDataTest
    {
        public string userName;
        public string Password;
        public string someOtherThing;
    }
    public List<PlayerDataTest> testList = new List<PlayerDataTest>(); //Replace this with BigParser-based techniques

    PlayerDataTest dirtyUsernameSearch(string targetUsername)
    {
        foreach (PlayerDataTest playerdata in testList)
        {
            if (playerdata.userName == targetUsername)
            {
                return playerdata;
            }
        }
        return null;
    }
    //BigParser stuff will probably be above this point




    //From here on, the functionality will probably remain mostly the same after integrating BigParser.

    //An RPC from clients that are asking to log in as a specific user
    public void ClientSentUsername(NetConnection sender, string username)
    {
        PlayerDataTest search = dirtyUsernameSearch(username);
        if (search != null)
        {
            //tell the client the username exists
            ServerGameLogic.instance.theServer.CallRPC("UsernameResponse", sender, true, "approved. This message should be ignored");
        }
        else
        {
            //tell the client the username does not exist
            ServerGameLogic.instance.theServer.CallRPC("UsernameResponse", sender, false, "FAILED TO RETRIEVE THAT USERNAME");

        }
    }

    //An RPC from clients that are attempting to log in as a user. 
    public void ClientLoginRequest(NetConnection sender, string username, string password)
    {
        PlayerDataTest search = dirtyUsernameSearch(username);
        if (search != null)
        {
            //if it exists, then check if the passwords match
            if (password == search.Password)
            {
                //if passwords match, then approve the login request.
                //More logic will go here to link up player data with the rest of the game systems
                ServerGameLogic.instance.theServer.CallRPC("LoginResponse", sender, true, "password is correct. This message should get ignored");
            }
            else
            {
                //if they dont match, deny the request
                ServerGameLogic.instance.theServer.CallRPC("LoginResponse", sender, false, "THAT PASSWORD IS INCORRECT");
            }
        }
        else
        {
            //if the username doesn't exist, then deny the request. If they managed to do this, then they are probably on a modded client
            ServerGameLogic.instance.theServer.CallRPC("LoginResponse", sender, false, "That username doesn't exist. You hacking?");

        }
    }


    public void ClientSignUpRequest(NetConnection sender, string username, string password)
    {
        //TODO: put signup logic here. Should probably wait
    }
}
