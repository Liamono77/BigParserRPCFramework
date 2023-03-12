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
    private List<object> syncUpdateParameters = new List<object>();

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        syncManager = safeSyncManager();
        NetAwake();
        syncManager.NetworkedStart(this, syncParameters.ToArray());

    }

    //scripts should override this when calling AddParameters to add whatever parameters should be sent to clients upon instantiation
    protected virtual void NetAwake()
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


    //scripts should override this with AddSyncUpdateParameters calls to add whatever parameters should be sent to clients during sync updates
    protected virtual void NetUpdate()
    {

    }
    protected void AddSyncUpdateParameters(params object[] parameters)
    {
        syncUpdateParameters.Clear();
        foreach (object p in parameters)
        {
            syncUpdateParameters.Add(p);
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
        NetUpdate();
        syncManager.SendSyncUpdate(this, syncUpdateParameters.ToArray());

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
