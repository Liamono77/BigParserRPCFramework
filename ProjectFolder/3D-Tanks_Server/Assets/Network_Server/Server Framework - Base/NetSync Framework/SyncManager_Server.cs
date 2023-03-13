using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

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

    public void SendSyncUpdate(NetSync_Server netSync, params object[] parameters)
    {
        BPDServer.instance.CallRPC("SyncUpdate", NetDeliveryMethod.Unreliable, netSync.ID, parameters);
    }

    public void NetworkedStart(NetSync_Server newNetSync, params object[] parameters)
    {
        if (!netSyncs.Contains(newNetSync))//make sure they're not already in the list
        {
            lastID++;
            newNetSync.SetID(lastID);
            netSyncs.Add(newNetSync);
            if (BPDServer.hasInitialized) //If the BPDServer hasn't initialized itself yet, then don't bother with the netinstantiation RPC because no clients will be connected.
            {
                CallNetInstantiation(newNetSync, parameters);
            }
        }
        else
        {
            NetLogger.LogError("attempted to add a NetSync that already exists");
        }
    }



    //keep these two overloads close to each other for ease of comparison. They're supposed to call the same RPC, except one should be for a specific client. My setup doens't seem to permit proper inheritance syntax. 
    void CallNetInstantiation(NetSync_Server newNetSync, params object[] parameters)
    {
        BPDServer.instance.CallRPC("NetInstantiation", newNetSync.prefabName, newNetSync.ID, new TransformInfo(newNetSync.transform), parameters);
    }
    void CallNetInstantiation(NetSync_Server newNetSync, NetConnection sender, params object[] parameters)
    {
        BPDServer.instance.CallRPC("NetInstantiation", sender, newNetSync.prefabName, newNetSync.ID, new TransformInfo(newNetSync.transform), parameters);
    }

    public void NetworkedDestroy(NetSync_Server netSync)
    {
        if (netSyncs.Contains(netSync))//make sure they're in the list
        {
            netSyncs.Remove(netSync);
            BPDServer.instance.CallRPC("NetDestroy", netSync.ID);
        }
        else
        {
            NetLogger.LogError("attempted to destroy a NetSync that doesn't exist");
        }
    }

    public void NetworkedEnable(NetSync_Server netSync)
    {
        if (BPDServer.hasInitialized)
        {
            if (netSyncs.Contains(netSync))//make sure they're in the list
            {
                //netSyncs.Remove(netSync);
                BPDServer.instance.CallRPC("NetEnable", netSync.ID);
            }
            else
            {
                NetLogger.LogError("attempted to enable a NetSync that doesn't exist");
            }
        }
    }
    public void NetworkedDisable(NetSync_Server netSync)
    {
        if (BPDServer.hasInitialized)
        {
            if (netSyncs.Contains(netSync))//make sure they're in the list
            {
                //netSyncs.Remove(netSync);
                BPDServer.instance.CallRPC("NetDisable", netSync.ID);
            }
            else
            {
                NetLogger.LogError("attempted to disable a NetSync that doesn't exist");
            }
        }
    }

    //RPC from clients that are attempting to resolve missing IDs
    public void ObjectRequest(NetConnection sender, int ID)
    {
        NetSync_Server netSync = GetNetSyncByID(ID);
        if (netSync != null)
        {
            NetLogger.LogWarning($"Player of ID {sender.RemoteUniqueIdentifier} has requested a networked object of ID {ID} (object name: {netSync.gameObject.name}). Sending information..");
            CallNetInstantiation(netSync, sender, netSync.getAwakeParameters());
        }
        else
        {
            NetLogger.LogWarning($"Player of ID {sender.RemoteUniqueIdentifier} has requested a networked object of ID {ID}, which doesn't exist. Sending information..");
            BPDServer.instance.CallRPC("FixMissingNetID", ID);
        }
    }

    //this is a resource-intensive lookup, only called occasionally
    NetSync_Server GetNetSyncByID(int ID)
    {
        foreach (NetSync_Server netSync in netSyncs)
        {
            if (netSync.ID == ID)
            {
                return netSync;
            }
        }
        NetLogger.LogWarning($"Failed to get network object of ID {ID}");
        return null;
    }
}
