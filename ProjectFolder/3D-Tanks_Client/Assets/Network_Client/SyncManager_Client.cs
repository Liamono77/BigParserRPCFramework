﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;

using System.Reflection;

public class SyncManager_Client : MonoBehaviour
{
    public static SyncManager_Client instance;

    public List<NetSync_Client> netSyncs = new List<NetSync_Client>();

    public List<int> missingSyncIDs = new List<int>();

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
            if (!missingSyncIDs.Contains(ID)) //If our missing ID list doesn't contain the ID, then request the object
            {
                NetLogger.LogWarning($"Recieved a sync update for object of unknown ID {ID}. Requesting object information from server...");
                BPDClient.instance.CallRPC("ObjectRequest", ID);
                missingSyncIDs.Add(ID);
            }
            else
            {
                //comment out this netlogger line for now. redundant information until we start seeing inexplicable network bugs.
                //NetLogger.LogWarning($"Recieved a sync update for object of unknown ID {ID}. Currently waiting for response from server...");

            }
        }
    }

    public void NetInstantiation(NetConnection server, string prefabName, int ID, TransformInfo transformInfo)
    {
        GameObject newObject = GameObject.Instantiate(Resources.Load<GameObject>(prefabName), transformInfo.position, transformInfo.rotation);
        NetSync_Client newNetSync = newObject.GetComponent<NetSync_Client>();
        newNetSync.ID = ID;
        netSyncs.Add(newNetSync);

        if (missingSyncIDs.Contains(ID))
        {
            NetLogger.LogWarning($"Missing ID {ID} has been recieved from server.");
            missingSyncIDs.Remove(ID);
        }
    }

    public void NetInstantiationPrototype(NetConnection server, string prefabName, int ID, TransformInfo transformInfo, params object[] parameters)
    {
        Debug.LogWarning("instantiation overload 1");
        GameObject newObject = GameObject.Instantiate(Resources.Load<GameObject>(prefabName), transformInfo.position, transformInfo.rotation);
        NetSync_Client newNetSync = newObject.GetComponent<NetSync_Client>();
        newNetSync.ID = ID;
        netSyncs.Add(newNetSync);
        newNetSync.InstantiationFunctionPrototype(parameters);

        if (missingSyncIDs.Contains(ID))
        {
            NetLogger.LogWarning($"Missing ID {ID} has been recieved from server.");
            missingSyncIDs.Remove(ID);
        }
    }
    //public void NetInstantiationPrototype(params object[] parameters)
    //{
    //    Debug.LogWarning("instantiation overload 2");
    //    NetConnection server = (NetConnection)parameters[0];
    //    string prefabName = (string)parameters[1];
    //    int ID = (int)parameters[2];
    //    TransformInfo transformInfo = (TransformInfo)parameters[3];





    //    GameObject newObject = GameObject.Instantiate(Resources.Load<GameObject>(prefabName), transformInfo.position, transformInfo.rotation);
    //    NetSync_Client newNetSync = newObject.GetComponent<NetSync_Client>();
    //    newNetSync.ID = ID;
    //    netSyncs.Add(newNetSync);
    //    newNetSync.InstantiationFunctionPrototype(parameters);

    //    if (missingSyncIDs.Contains(ID))
    //    {
    //        NetLogger.LogWarning($"Missing ID {ID} has been recieved from server.");
    //        missingSyncIDs.Remove(ID);
    //    }
    //}

    public void FixMissingNetID(NetConnection server, int ID)
    {
        NetLogger.LogWarning($"Server has informed this client that missing ID {ID} no longer exists in its records. Removing from missing ID list...");
        missingSyncIDs.Remove(ID);
    }

    public void NetDestroy(NetConnection server, int ID)
    {
        NetSync_Client netSync = GetNetSyncByID(ID);
        if (netSync != null)
        {
            NetLogger.Log($"Recieved destroy message for object of ID {ID}, name {netSync.gameObject.name}");
            netSyncs.Remove(netSync);
            Destroy(netSync.gameObject);
        }
    }

    public void NetEnable(NetConnection server, int ID)
    {
        NetSync_Client netSync = GetNetSyncByID(ID);
        if (netSync != null)
        {
            NetLogger.Log($"Recieved enable message for object of ID {ID}, name {netSync.gameObject.name}");
            //netSyncs.Remove(netSync);
            //Destroy(netSync.gameObject);
            netSync.gameObject.SetActive(true);
        }
    }
    public void NetDisable(NetConnection server, int ID)
    {
        NetSync_Client netSync = GetNetSyncByID(ID);
        if (netSync != null)
        {
            NetLogger.Log($"Recieved disable message for object of ID {ID}, name {netSync.gameObject.name}");
            //netSyncs.Remove(netSync);
            //Destroy(netSync.gameObject);
            netSync.gameObject.SetActive(false);
        }
    }



    //WARNING: this is a resource-intensive lookup that will happen every Sync Update. If profiler points to this script as a high consumer, then consider this a possible culprit
    public NetSync_Client GetNetSyncByID(int ID)
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
