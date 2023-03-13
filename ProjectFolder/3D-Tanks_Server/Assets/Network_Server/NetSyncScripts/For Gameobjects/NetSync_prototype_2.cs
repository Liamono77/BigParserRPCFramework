using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototype_2 : NetSync_prototype
{
    public bool lolBool = true;
    public int lolInt = 2839;
    public string lol1 = "THE SECOND PROTOTYPE LOLOLOL";
    public string lol2 = "aiywbckaas";

    protected override void SetParameters()
    {
        base.SetParameters();
        AddNetAwakeParameters(lolBool, lolInt, lol1, lol2);
    }
}
