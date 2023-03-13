using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncStandard : NetSync_Server
{
    protected override void SetParameters()
    {
        base.SetParameters();
    }
    protected override void NetUpdate()
    {
        base.NetUpdate();
        AddNetUpdateParameters(new TransformInfo(transform));
    }
}
