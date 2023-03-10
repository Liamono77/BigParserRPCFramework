using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

public class SyncManager_Client : MonoBehaviour
{
    public static SyncManager_Client instance;

    public List<NetSync_Client> netSyncs = new List<NetSync_Client>();

    private void Awake()
    {
        instance = this;
    }

    //RPC for syncing up network objects
    public void SyncUpdate(NetConnection server, int ID, TransformInfo transformInfo)
    {
        NetSync_Client netSyncToSync = GetNetSyncByID(ID);
        if (netSyncToSync != null)
        {
            netSyncToSync.transform.position = transformInfo.position;
            netSyncToSync.transform.rotation = transformInfo.rotation;
        }
        else
        {
            //Debug.Log("load a net sync object");
        }
    }

    public void NetInstantiation(NetConnection server, string prefabName, int ID, TransformInfo transformInfo)
    {
        GameObject newObject = GameObject.Instantiate(Resources.Load<GameObject>(prefabName), transformInfo.position, transformInfo.rotation);
        NetSync_Client newNetSync = newObject.GetComponent<NetSync_Client>();
        newNetSync.ID = ID;
        netSyncs.Add(newNetSync);
    }

    public void NetDestroy(NetConnection server, int ID)
    {
        NetSync_Client netSync = GetNetSyncByID(ID);
        if (netSync != null)
        {
            netSyncs.Remove(netSync);
            Destroy(netSync.gameObject);
        }
    }


    //WARNING: this is a resource-intensive lookup that will happen every Sync Update. If profiler points to this script as a high consumer, then consider this a possible culprit
    NetSync_Client GetNetSyncByID(int ID)
    {
        foreach (NetSync_Client netSync in netSyncs)
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
