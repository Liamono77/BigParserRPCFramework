using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NET SYNC (SERVER)
public class NetSync_Server : MonoBehaviour
{
    public SyncManager_Server syncManager;
    public int ID;
    public string prefabName; //CONSIDER REPLACING THIS WITH AUTOMATION

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        //syncManager = SyncManager_Server.instance;
        syncManager = safeSyncManager();
        //Debug.Log($"awake {this.gameObject.name}, manager {syncManager.gameObject.name}");
        syncManager.NetworkedStart(this);
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
        //syncManager.NetworkedStart(this);

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
        //if (BPDServer.hasInitialized)
        //{
        //    syncManager.NetworkedEnable(this);
        //}

        syncManager.NetworkedEnable(this);
    }
    protected virtual void OnDisable()
    {
        //if(BPDServer.hasInitialized)
        //{
        //    syncManager.NetworkedDisable(this);
        //}

        syncManager.NetworkedDisable(this);

    }

    public void SetID(int newID)
    {
        ID = newID;
    }

    //public IEnumerable emergencyDelayThingy()
    //{
    //    yield return new WaitForSeconds(1);

    //    NetLogger.LogWarning("emergencyDelay");
    //    //yield return null;
    //}
}
