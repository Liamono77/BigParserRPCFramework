using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lidgren.Network;
using UnityEngine.UI;

//LOGIN MANAGER
//This script primarily controls login screen UI and login RPC calls. 
//Bit of a programming fail on my part to try cramming all login-specific logic into this single script. Consider replacing with multiple scripts in the future.
//WRITTEN BY LIAM SHELTON
public class LoginManager : MonoBehaviour
{
    //This state machine enum defines the various states of signin. 
    public LogInState logInState = LogInState.usernameEntry;
    public enum LogInState
    {
        usernameEntry,
        passwordEntry,
        signup,
    }

    //This block contains references to username entry UI
    public GameObject usernameScreen; //The GameObject containing username entry UI
    public InputField usernameText;
    public GameObject usernameFailMessage;

    //This block contains references to password entry UI
    public GameObject passwordScreen; //The GameObject containing password entry UI
    public InputField passwordText;
    public GameObject passwordFailMessage;
    public Text logInAsUsernameMessage;

    //This block contains references to signup UI
    public GameObject signUpScreen; //The GameObject containing signup UI
    public InputField newUsernameField;
    public InputField newPasswordField;
    public InputField secondPasswordField;
    public Text signupFailMessage;

    //This block contains references to the UI indicator that informs clients if their login screen is waiting for a response from the server
    public GameObject waitingForResponseIndicator;
    public bool waitingForServerResponse;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        usernameScreen.SetActive(logInState == LogInState.usernameEntry);
        passwordScreen.SetActive(logInState == LogInState.passwordEntry);
        signUpScreen.SetActive(logInState == LogInState.signup);
        waitingForResponseIndicator.SetActive(waitingForServerResponse);

        logInAsUsernameMessage.text = $"attempting to log in as user {usernameText.text}";
    }

    //RPC from server letting us know we've successfully logged in
    public void LoginResponse(NetConnection server, bool approved, string failMessage)
    {
        waitingForServerResponse = false;

        if (approved)
        {
            ClientGameLogic.instance.SwitchToLobby();
        }
        else
        {
            passwordFailMessage.SetActive(true);
            passwordFailMessage.GetComponent<Text>().text = failMessage;
        }
    }

    //RPC from server letting us know that our entered username exists in its records.
    public void UsernameResponse(NetConnection server, bool usernameExists, string failMessage)
    {
        waitingForServerResponse = false;

        if (usernameExists)
        {
            logInState = LogInState.passwordEntry;
        }
        else
        {
            usernameFailMessage.SetActive(true);
            usernameFailMessage.GetComponent<Text>().text = failMessage;
        }
    }

    //Call this through the continue button to send a username entry to the server.
    public void SendUsernameToServer()
    {
        waitingForServerResponse = true;
        usernameFailMessage.SetActive(false);
        ClientGameLogic.instance.theClient.CallRPC("ClientSentUsername", usernameText.text);
    }

    //Call this through the login button to send a login request to the server (will send username and password together)
    public void SendLoginRequestToServer()
    {
        waitingForServerResponse = true;
        passwordFailMessage.SetActive(false);
        ClientGameLogic.instance.theClient.CallRPC("ClientLoginRequest", usernameText.text, passwordText.text);

    }

    //call this through the BACK ui button to go back to the username screen
    public void BackButton()
    {
        logInState = LogInState.usernameEntry;
        passwordFailMessage.SetActive(false);
        usernameFailMessage.SetActive(false);
        waitingForServerResponse = false;
    }

    //call this through the SignUp switch button to bring up the signup menu
    public void SwitchToSignUpMode()
    {
        logInState = LogInState.signup;
        passwordFailMessage.SetActive(false);
        usernameFailMessage.SetActive(false);
        waitingForServerResponse = false;
    }

    //call this through a Register button of some sort to send a signup request to the server (UNIMPLEMENTED ON SERVER--WAITING FOR BIGPARSER INTEGRATION)
    public void SendSignUpRequest()
    {
        if (newPasswordField.text == secondPasswordField.text)
        {
            //if passwords both match, then proceed to send the signup
            waitingForServerResponse = true;
            signupFailMessage.gameObject.SetActive(false);
            ClientGameLogic.instance.theClient.CallRPC("ClientSignUpRequest", newUsernameField.text, newPasswordField.text);
        }
        else
        {
            signupFailMessage.gameObject.SetActive(true);
            signupFailMessage.text = "passwords do not match";
        }


    }

    //RPC from server confirming or denying sign up request. TODO: set up local storage of username and password
    public void SignUpResponse(NetConnection server, bool approved, string failMessage)
    {
        waitingForServerResponse = false;

        if (approved)
        {
            usernameText.text = newUsernameField.text;
            signupFailMessage.gameObject.SetActive(false);

        }
        else
        {
            signupFailMessage.gameObject.SetActive(true);
            signupFailMessage.text = failMessage;
        }
    }

    //test rpc from when the RPC framework was new
    public void TestLoginRPC(NetConnection server)
    {
        Debug.Log("LOGINRPCCALLED");
    }
}
