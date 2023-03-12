using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototype : NetSync_Server
{
    public float health = 10;
    public string someString = "PROTOTYPELOLOLOL";

    protected override void Awake()
    {
        //base.Awake();
        syncManager = safeSyncManager();
        syncManager.NetworkedStartPrototype(this, health, someString);
    }
}
