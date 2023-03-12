using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NetSync_Client : MonoBehaviour
{
    public int ID;
    protected virtual void Awake()
    {
        
    }

    // Start is called before the first frame update
    protected virtual void Start()
    {
        
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        
    }


    //Called from external script
    public void NetInstantiation(params object[] parameters)
    {
        List<object> parametersToProcess = parameters.ToList<object>();
        ProcessInstantiationParameters(ref parametersToProcess);
    }

    //Inheriting scripts should override this with their own logic on a per-variable basis
    protected virtual void ProcessInstantiationParameters(ref List<object> list)
    {

    }

    public void NetUpdate(params object[] parameters)
    {
        List<object> parametersToProcess = parameters.ToList<object>();
        ProcessNetUpdateParameters(ref parametersToProcess);
    }

    protected virtual void ProcessNetUpdateParameters(ref List<object> list)
    {

    }

    protected void Deque<T>(ref T variable, List<object> parameters)
    {
        if (variable.GetType() == parameters[0].GetType())
        {
            variable = (T)parameters[0];
            parameters.RemoveAt(0);
        }
    }
}



