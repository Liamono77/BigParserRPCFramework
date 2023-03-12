using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncStandard : NetSync_Client
{
    protected override void NetAwake(ref List<object> list)
    {
        base.NetAwake(ref list);
    }

    protected override void NetUpdate(ref List<object> list)
    {
        base.NetUpdate(ref list);

        TransformInfo transformInfo = new TransformInfo();
        Deque(ref transformInfo, list);
        transform.position = transformInfo.position;
        transform.rotation = transformInfo.rotation;

    }
}
