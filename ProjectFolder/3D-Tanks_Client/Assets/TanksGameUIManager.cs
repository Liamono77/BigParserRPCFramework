using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TanksGameUIManager : MonoBehaviour
{
    public RespawnUIManager respawnUI;
    public GameObject gameplayUI;

    //public GameObject respawnMenu;
    // Start is called before the first frame update
    void Start()
    {
        //TankClientUniversal.instance.serverJoinCallback += 
    }

    // Update is called once per frame
    void Update()
    {
        respawnUI.gameObject.SetActive((TankClientUniversal.instance.serverState == TankClientUniversal.ServerState.playing) && (TankClientUniversal.instance.playerState == TankClientUniversal.PlayerState.Spawning));
    }

    //public void SetServerInfoScreen
}
