using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ServerInfoScreen : UIScreen
{
    public Text m_serverName;
    public Text m_gameMode;
    public Text m_description;
    public Text m_serverState;

    public override void ManagedStart()
    {
        base.ManagedStart();
        TankClientUniversal.instance.serverJoinCallback += setInformation;
    }
    public void setInformation (string serverName, string gameMode, string description, string serverState)
    {
        NetLogger.Log($"UI: serverinfoscreen attempting to update its server information...");
        m_serverName.text = serverName;
        m_gameMode.text = $"GAMEMODE: {gameMode}";
        m_description.text = description;
        m_serverState.text = $"SERVER STATUS: {serverState}";
    }
}
