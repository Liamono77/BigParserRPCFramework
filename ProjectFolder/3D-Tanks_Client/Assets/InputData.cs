using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//INPUT DATA
//Treat this class as a universal representation of player data that should be sent to the server at high frequencies
//The server will use this data to drive the tanks, so the higher the call frequency, the more precise movement will feel for players.
//Try to keep this data as concise as possible. Its size may affect network traffic significantly
[System.Serializable]
public class InputData
{
    public Vector2 moveDirection; //Treat this like a stick input (WASD on keyboard, leftstick on controller)
    public Vector3 targetPosition;//Treat this as the location the player's tank should aim at
    public bool fire;//Treat this as a signal for firing the main weapon
}
