using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//NET SYNC (client)
//This is an abstract class to 
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
    public void ManagedAwake(params object[] parameters)
    {
        List<object> parametersToProcess = parameters.ToList<object>();
        NetAwake(ref parametersToProcess);
    }

    //Inheriting scripts should override this with their own logic on a per-variable basis
    protected virtual void NetAwake(ref List<object> list)
    {

    }


    //Called from external script
    public void ManagedUpdate(params object[] parameters)
    {
        List<object> parametersToProcess = parameters.ToList<object>();
        NetUpdate(ref parametersToProcess);
    }

    //Inheriting scripts should override this with their own logic on a per-variable basis
    protected virtual void NetUpdate(ref List<object> list)
    {

    }


    //Use this when setting variables to ensure that inheriting classes don't need to pay attention to their parent's variable count
    protected void Deque<T>(ref T variable, List<object> parameters)
    {
        if (variable.GetType() == parameters[0].GetType())
        {
            variable = (T)parameters[0];
            parameters.RemoveAt(0);
        }
    }
}



