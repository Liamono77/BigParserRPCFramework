using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//screw it, this script is getting promoted to general playmode menu logic
public class RespawnScreen : UIScreen
{
    public GameObject waitingForPlayersScreen;

    //public GameObject killCamUI;
    //public GameObject 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        waitingForPlayersScreen.SetActive(TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.waitingForPlayers);
    }
}
