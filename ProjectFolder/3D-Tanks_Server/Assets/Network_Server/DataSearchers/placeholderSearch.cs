using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//the placeholder datasearcher that uses a local list of players.
public class placeholderSearch : DataSearcher
{
    public List<PlayerData> testList = new List<PlayerData>(); //Replace this with BigParser-based techniques

    public override PlayerData UsernameSearch(string targetUsername)
    {
        foreach (PlayerData playerdata in testList)
        {
            if (playerdata.username == targetUsername)
            {
                return playerdata;
            }
        }
        return null;
    }
}
