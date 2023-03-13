using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncTankTemp : NetSyncStandard
{
    //public List<MeshRenderer> meshRenderers = new List<MeshRenderer>();
    public Color myColor;

    protected override void NetAwake()
    {
        base.NetAwake();
        myColor = Random.ColorHSV();
    }

    protected override void SetParameters()
    {
        base.SetParameters();
        AddNetAwakeParameters(myColor.r, myColor.g, myColor.b);
    }

}
