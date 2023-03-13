using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaitingForPlayersUI : UIBase
{
    public Text playerCountText;
    public Text playerListText;
    public override void ManagedStart()
    {
        base.ManagedStart();

    }

    private void Update()
    {
        playerCountText.text = $"{TankClientUniversal.instance.connectedPlayers.Count}";


        //Sync the display with the names list
        playerListText.text = "";
        foreach (TankClientUniversal.PlayerConnectionInfo connection in TankClientUniversal.instance.connectedPlayers)
        {
            playerListText.text = $"{playerListText.text + connection.username}, ";
        }
    }

    //private void OnEnable()
    //{
    //    if (TankClientUniversal.instance.serverState != TankClientUniversal.ServerState.nullState)
    //    {
    //        BPDClient.instance.CallRPC("")
    //    }
    //}
}
