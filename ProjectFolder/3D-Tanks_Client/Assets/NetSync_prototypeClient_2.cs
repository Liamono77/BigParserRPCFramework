using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSync_prototypeClient_2 : NetSync_Client
{
    public bool someBoolean;
    public int someInt;
    public string aString1;
    public string aString2;
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
        someBoolean = (bool)parameters[0];
        someInt = (int)parameters[1];
        aString1 = (string)parameters[2];
        aString2 = (string)parameters[3];


        //GameObject.Instantiate(Resources.Load<GameObject>(someString), transform.position, transform.rotation);
    }
}
