using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototypeClient : NetSync_Client
{
    public float health;
    public string someString;

    protected override void ProcessInstantiationParameters(ref List<object> list)
    {
        base.ProcessInstantiationParameters(ref list);
        Deque(ref health, list);
        Deque(ref someString, list);
    }
}
