using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototypeClient : NetSync_Client
{
    public float health;
    public string someString;
    //public override void InstantiationFunctionPrototype(params object[] parameters)
    //{
    //    base.InstantiationFunctionPrototype(parameters);
    //    health = (float)parameters[0];
    //    someString = (string)parameters[1];

    //    //GameObject.Instantiate(Resources.Load<GameObject>(someString), transform.position, transform.rotation);
    //}
    public override void InstantiationFunctionPrototype(params object[] parameters)
    {
        base.InstantiationFunctionPrototype(parameters);
        health = (float)parameters[0];
        someString = (string)parameters[1];

        //GameObject.Instantiate(Resources.Load<GameObject>(someString), transform.position, transform.rotation);
    }
}
