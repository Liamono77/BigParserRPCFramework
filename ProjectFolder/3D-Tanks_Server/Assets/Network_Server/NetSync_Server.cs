using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NET SYNC (SERVER)
public class NetSync_Server : MonoBehaviour
{
    public SyncManager_Server syncManager;
    public int ID;
    public string prefabName; //CONSIDER REPLACING THIS WITH AUTOMATION

    private List<object> syncParameters = new List<object>();

    public bool usePrototype;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        syncManager = safeSyncManager();
        PreAwake();

        if (usePrototype == false)
        {
            syncManager.NetworkedStart(this);

        }
        else
        {
            syncManager.NetworkedStartPrototype(this, syncParameters.ToArray());
        }

    }

    //scripts should override this when calling AddParameters to add whatever parameters will 
    protected virtual void PreAwake()
    {

    }

    //Inheriting scripts should call this to specify parameters that should be sent as part of a networked instantiation
    protected void AddInstantiationParameters(params object[] parameters)
    {
        foreach (object p in parameters)
        {
            syncParameters.Add(p);
        }
    }

    protected SyncManager_Server safeSyncManager()
    {
        if (syncManager != null)
        {
            return syncManager;
        }
        if (SyncManager_Server.instance != null)
        {
            return SyncManager_Server.instance;
        }
        else
        {
            Debug.LogWarning("sync object has probably called Awake before the syncmanager's Awake. Running GameObject.Find()...");
            return (FindObjectOfType<SyncManager_Server>());
        }
    }

    //Override this with specialized logic. Try to minimize the data 
    protected virtual void SendSyncUpdate()
    {

    }
    protected virtual void SendNetInstantiation()
    {

    }

    protected virtual void Start()
    {

    }

    // Update is called once per frame
    protected virtual void Update()
    {
        RequestSyncUpdate();
    }

    protected virtual void RequestSyncUpdate()
    {
        syncManager.SendSyncUpdate(this);
    }

    protected virtual void OnDestroy()
    {
        syncManager.NetworkedDestroy(this);

    }
    protected virtual void OnEnable()
    {

        syncManager.NetworkedEnable(this);
    }
    protected virtual void OnDisable()
    {

        syncManager.NetworkedDisable(this);

    }

    public void SetID(int newID)
    {
        ID = newID;
    }

}
