using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncStandard : NetSync_Server
{
    protected override void NetAwake()
    {
        base.NetAwake();
    }
    protected override void NetUpdate()
    {
        base.NetUpdate();
        AddSyncUpdateParameters(new TransformInfo(transform));
    }
}
