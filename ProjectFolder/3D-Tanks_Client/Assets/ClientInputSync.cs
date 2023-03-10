using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

//Try to use this script for physically-based controls that should be blasted to the server. Do everything else in specific RPC calls.
public class ClientInputSync : MonoBehaviour
{
    public InputData currentInputData;

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
