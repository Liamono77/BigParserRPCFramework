using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototype : NetSync_Server
{
    public float health = 10;
    public string someString = "PROTOTYPELOLOLOL";

    protected override void PreAwake()
    {
        base.PreAwake();
        AddInstantiationParameters(health, someString);
    }
}
