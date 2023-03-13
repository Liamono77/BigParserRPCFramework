using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//screw it, this script is getting promoted to general playmode menu logic
public class RespawnScreen : UIScreen
{
    public GameObject waitingForPlayersScreen;

    public float respawnTimer; //A local variable to help display time until respawn. This can be hacked by the user and would not enable faster spawning (server watches its own timer).

    public GameObject trueRespawnScreen;
    public Text respawnTimeDisplay;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public override void ManagedStart()
    {
        base.ManagedStart();
        TankClientUniversal.instance.deathCallback += SetTimer;
    }

    // Update is called once per frame
    void Update()
    {
        waitingForPlayersScreen.SetActive(TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.waitingForPlayers);
        trueRespawnScreen.SetActive((TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.playing) || (TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.preGame));
        //respawnTimeDisplay.text = $"{Mathf.Round(respawnTimer)}";
        respawnTimeDisplay.text = $"{Mathf.Round(respawnTimer - Time.time)}";

        localRespawnLimiter();
        //BPDClient.instance.CallRPC("PlayerSpawnRequest", Lidgren.Network.NetDeliveryMethod.Unreliable);

    }

    void SetTimer(float respawnTime, int killerSyncID, int killerPlayerID, string message)
    {
        respawnTimer = Time.time + respawnTime;
    }

    //This function puts a leash on how frequently the client will make respawn requests when they are dead. Note that the server makes its own checks for player respawn time, so this is not a security flaw
    void localRespawnLimiter()
    {
        if (TankClientUniversal.instance.playerState == TankClientUniversal.PlayerState.Spawning)
        {
            if (TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.preGame || TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.playing)
            {
                if (respawnTimer < Time.time)
                {
                    BPDClient.instance.CallRPC("PlayerSpawnRequest");
                    //respawnTimer = 1f;
                }
                else
                {
                    //respawnTimer = respawnTimer - Time.deltaTime;
                }
            }
        }
    }
}
