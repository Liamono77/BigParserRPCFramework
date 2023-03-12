using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncStandard : NetSync_Client
{
    protected override void ProcessInstantiationParameters(ref List<object> list)
    {
        base.ProcessInstantiationParameters(ref list);
    }

    protected override void ProcessNetUpdateParameters(ref List<object> list)
    {
        base.ProcessNetUpdateParameters(ref list);

        TransformInfo transformInfo = new TransformInfo();
        Deque(ref transformInfo, list);
        transform.position = transformInfo.position;
        transform.rotation = transformInfo.rotation;

    }
}
