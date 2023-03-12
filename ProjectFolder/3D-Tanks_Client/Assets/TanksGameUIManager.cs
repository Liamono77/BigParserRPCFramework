using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TanksGameUIManager : MonoBehaviour
{
    public RespawnUIManager respawnUI;
    public GameObject gameplayUI;

    public List<UIBase> UIBases = new List<UIBase>();

    //public GameObject respawnMenu;
    // Start is called before the first frame update
    void Start()
    {
        //TankClientUniversal.instance.serverJoinCallback += 
        UIBase[] L = GetComponentsInChildren<UIBase>(true);
        foreach (UIBase ui in L)
        {
            UIBases.Add(ui);
        }

        foreach (UIBase ui in UIBases)
        {
            ui.ManagedStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        respawnUI.gameObject.SetActive((TankClientUniversal.instance.serverState != TankClientUniversal.ServerState.nullState) && (TankClientUniversal.instance.playerState == TankClientUniversal.PlayerState.Spawning));
    }

    //public void SetServerInfoScreen
}
