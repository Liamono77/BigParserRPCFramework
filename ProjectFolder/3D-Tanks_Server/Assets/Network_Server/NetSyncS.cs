using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//NET SYNC (SERVER)
public class NetSyncS : MonoBehaviour
{
    public SyncManagerServer syncManager;
    public int ID;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        syncManager = SyncManagerServer.instance;
        syncManager.NetworkedStart(this);
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

    public void SetID(int newID)
    {
        ID = newID;
    }
}
