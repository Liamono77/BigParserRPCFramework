using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public LogInState logInState = LogInState.usernameEntry;
    public enum LogInState
    {
        usernameEntry,
        passwordEntry,
    }
    public GameObject usernameScreen;
    public GameObject passwordScreen;

    public Text usernameText;
    public GameObject usernameFailMessage;

    public GameObject waitingForResponseIndicator;
    public bool waitingForServerResponse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //usernameScreen.SetActive(logInState == LogInState.usernameEntry);
        //passwordScreen.SetActive(logInState == LogInState.passwordEntry);
        //waitingForResponseIndicator.SetActive(waitingForServerResponse);
    }

    public void UsernameResponse(NetConnection server, bool usernameExists)
    {
        waitingForServerResponse = false;

        if (usernameExists)
        {
            logInState = LogInState.passwordEntry;
        }
        else
        {
            usernameFailMessage.SetActive(true);
        }
    }

    public void SendUsernameToServer()
    {
        waitingForServerResponse = true;
        usernameFailMessage.SetActive(false);
        ClientGameLogic.instance.theClient.CallRPC("ClientSentUsername", usernameText.text);
    }

    public void TestLoginRPC(NetConnection server)
    {
        Debug.Log("LOGINRPCCALLED");
    }
}
