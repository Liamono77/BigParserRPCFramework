using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//INPUT DATA
//Treat this class as a universal representation of player data that should be sent to the server at high frequencies
//The server will use this data to drive the tanks, so the higher the call frequency, the more precise movement will feel for players.
//Try to keep this data as concise as possible. Its size may affect network traffic significantly
[System.Serializable]
public class TankInputData
{
    public Vector2 moveDirection; //Treat this like a stick input (WASD on keyboard, leftstick on controller)
    public Vector3 targetPosition;//Treat this as the location the player's tank should aim at
    public bool fire;//Treat this as a signal for firing the main weapon
}


//Try to use this script for physically-based controls that should be blasted to the server. Do everything else in specific RPC calls.
public class TankInputUpdator : MonoBehaviour
{
    public TankInputData currentInputData;

    private float inputUpdateTimer; //a timer variable to avoid coroutine use
    public float inputUpdateDelay = 0.016f; //the time in seconds this script should wait for between input updates. If set to 0, it will happen every frame.

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MonitorUpdateTimer();
    }

    void MonitorUpdateTimer()
    {
        if (inputUpdateTimer < Time.time)
        {
            inputUpdateTimer = Time.time + inputUpdateDelay;
            SendInputUpdate();
        }
    }

    void SendInputUpdate()
    {
        BPDClient.instance.CallRPC("InputUpdate", NetDeliveryMethod.Unreliable, currentInputData.moveDirection.x, currentInputData.moveDirection.y, currentInputData.targetPosition, currentInputData.fire);
    }
}
