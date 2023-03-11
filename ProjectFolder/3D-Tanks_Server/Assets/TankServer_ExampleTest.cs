using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TankServer_ExampleTest : MonoBehaviour
{
    public TankServerUniversal tankServer;

    public float radius = 5f;
    // Start is called before the first frame update
    void Start()
    {
        tankServer.onPlayerSpawn += RandomSpawn;
    }

    public void RandomSpawn(PlayerConnection player)
    {
        //TankServerPlayerData playerData = (player.serverData as TankServerPlayerData);
        //playerData. = GameObject.Instantiate<GameObject>(tankServer);

        TankServerPlayerData playerData = (player.serverData as TankServerPlayerData);
        playerData.currentTank.transform.position = new Vector3(Random.Range(-radius, radius), 0, Random.Range(-radius, radius));

        //do the thing for setting tank manager references
        Complete.TankMovement tankMovement = playerData.currentTank.GetComponent<Complete.TankMovement>();
        Complete.TankShooting tankShooting = playerData.currentTank.GetComponent<Complete.TankShooting>();
        tankMovement.tankInputData = player.inputData as TankInputData;
        tankShooting.tankInputData = player.inputData as TankInputData;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
