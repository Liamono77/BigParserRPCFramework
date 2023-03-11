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
    public DataSearcher searcher;

    //An RPC from clients that are asking to log in as a specific user
    public void ClientSentUsername(NetConnection sender, string username)
    {
        PlayerData search = searcher.UsernameSearch(username);
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
        PlayerData search = searcher.UsernameSearch(username);
        if (search != null)
        {
            //if it exists, then check if the passwords match
            if (password == search.password)
            {
                //if passwords match, then approve the login request.
                //More logic will go here to link up player data with the rest of the game systems
                ServerGameLogic.AddPlayerConnection(sender, $"Login approved", -1, username);
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
