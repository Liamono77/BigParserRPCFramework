using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncManager_Server : MonoBehaviour
{
    public List<NetSync_Server> netSyncs = new List<NetSync_Server>();

    public static SyncManager_Server instance; //Singleton reference, for independence

    private int lastID = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SendSyncUpdate(NetSync_Server netSync)
    {
        BPDServer.instance.CallRPC("SyncUpdate", netSync.ID, new TransformInfo(netSync.transform));
    }

    public void NetworkedStart(NetSync_Server newNetSync)
    {
        if (!netSyncs.Contains(newNetSync))//make sure they're not already in the list
        {
            lastID++;
            newNetSync.SetID(lastID);
            netSyncs.Add(newNetSync);

        }
        else
        {
            NetLogger.LogError("attempted to add a NetSync that already exists");
        }
    }

    public void NetworkedDestroy(NetSync_Server netSync)
    {
        if (netSyncs.Contains(netSync))//make sure they're not already in the list
        {
            netSyncs.Remove(netSync);
        }
        else
        {
            NetLogger.LogError("attempted to destroy a NetSync that doesn't exist");
        }
    }

    //public void AddNetSync(NetSyncS newNetSync, int newID)
    //{
    //    if (!netSyncs.Contains(newNetSync))//make sure they're not already in the list
    //    {
    //        newNetSync.SetID(newID);
    //        netSyncs.Add(newNetSync);
    //        CallAddSyncRPC(newNetSync, newID);
    //    }
    //    else
    //    {
    //        NetLogger.LogError("attempted to add a NetSync that already exists");
    //    }
    //}


    //protected virtual void CallAddSyncRPC(NetSyncS newNetSync, int newID)
    //{

    //}
}
