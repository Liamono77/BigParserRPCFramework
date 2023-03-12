using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototypeClient_2 : NetSync_prototypeClient
{
    public bool someBoolean;
    public int someInt;
    public string aString1;
    public string aString2;

    protected override void ProcessInstantiationParameters(ref List<object> list)
    {
        base.ProcessInstantiationParameters(ref list);
        Deque(ref someBoolean, list);
        Deque(ref someInt, list);
        Deque(ref aString1, list);
        Deque(ref aString2, list);


    }
}
