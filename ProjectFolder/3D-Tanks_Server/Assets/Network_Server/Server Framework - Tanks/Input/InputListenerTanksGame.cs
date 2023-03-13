using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//TANK INPUT DATA
//Treat this class as a universal representation of player data that should be sent to the server at high frequencies
//The server project will use this data to drive the tanks
[System.Serializable]
public class TankInputData
{
    public Vector2 moveDirection; //Treat this like a stick input (WASD on keyboard, leftstick on controller)
    public Vector3 targetPosition;//Treat this as the location the player's tank should aim at
    public bool fire;//Treat this as a signal for firing the main weapon
}
public class InputListenerTanksGame : MonoBehaviour
{
    public List<TankInputData> tankInputInstances = new List<TankInputData>();
    protected void Start()
    {
        ServerGameLogic.instance.playerAddedCallback += SetPlayerInputData;
    }

    public void InputUpdate(NetConnection sender, float moveX, float moveY, Vector3 targetPosition, bool fire)
    {
        PlayerConnection player = ServerGameLogic.GetPlayer(sender);
        if (player != null)
        {
            (player.inputData as TankInputData).moveDirection.x = moveX;
            (player.inputData as TankInputData).moveDirection.y = moveY;
            (player.inputData as TankInputData).targetPosition = targetPosition;
            (player.inputData as TankInputData).fire = fire;
        }

    }

    public void SetPlayerInputData(PlayerConnection player)
    {
        TankInputData newTankInput = new TankInputData();
        player.inputData = newTankInput;
        tankInputInstances.Add(newTankInput);
    }
}
