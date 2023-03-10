using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
public abstract class InputListener_Server<T> : MonoBehaviour
{
    protected ServerGameLogic serverLogic;
    protected virtual void Start()
    {
        ServerGameLogic.instance.playerAddedCallback += SetPlayerInputData;
        serverLogic = ServerGameLogic.instance;
    }

    public virtual void UpdateInputs(NetConnection sender, params object[] parameters)
    {
        //foreach ()
    }

    protected virtual void SetPlayerInputData(PlayerConnection player)
    {

    }
}
