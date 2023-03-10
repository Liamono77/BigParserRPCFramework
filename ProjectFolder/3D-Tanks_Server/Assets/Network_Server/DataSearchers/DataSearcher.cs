using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//An abstract class representing scripts that find player data via a username search
public abstract class DataSearcher : MonoBehaviour
{
    public virtual PlayerData UsernameSearch(string targetUsername) 
    {
        return null;
    }
}

//Class to represent player data. Should be extendable
[System.Serializable]
public class PlayerData
{
    public string username;
    public string password;
}