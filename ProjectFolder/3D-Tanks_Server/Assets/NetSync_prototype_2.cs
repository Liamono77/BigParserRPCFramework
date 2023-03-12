using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototype_2 : NetSync_Server
{
    public bool lolBool = true;
    public int lolInt = 2839;
    public string lol1 = "THE SECOND PROTOTYPE LOLOLOL";
    public string lol2 = "aiywbckaas";

    protected override void Awake()
    {
        //base.Awake();
        syncManager = safeSyncManager();
        syncManager.NetworkedStartPrototype(this, lolBool, lolInt, lol1, lol2);
    }
}
