using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototypeClient : NetSync_Client
{
    public float health;
    public string someString;

    protected override void NetAwake(ref List<object> list)
    {
        base.NetAwake(ref list);
        Deque(ref health, list);
        Deque(ref someString, list);
    }

    protected override void NetUpdate(ref List<object> list)
    {
        base.NetUpdate(ref list);
        Deque(ref health, list);
        Deque(ref someString, list);
    }
}
