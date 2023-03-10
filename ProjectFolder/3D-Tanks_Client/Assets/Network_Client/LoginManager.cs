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
        signup,
    }
    public GameObject usernameScreen;
    public InputField usernameText;
    public GameObject usernameFailMessage;

    public GameObject passwordScreen;
    public InputField passwordText;
    public GameObject passwordFailMessage;
    public Text logInAsUsernameMessage;

    public GameObject signUpScreen;
    public InputField newUsernameField;
    public InputField newPasswordField;
    public InputField secondPasswordField;
    public Text signupFailMessage;

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

    //Call this through the continue button
    public void SendUsernameToServer()
    {
        waitingForServerResponse = true;
        usernameFailMessage.SetActive(false);
        ClientGameLogic.instance.theClient.CallRPC("ClientSentUsername", usernameText.text);
    }

    //Call this through the login button
    public void SendLoginRequestToServer()
    {
        waitingForServerResponse = true;
        passwordFailMessage.SetActive(false);
        ClientGameLogic.instance.theClient.CallRPC("ClientLoginRequest", usernameText.text, passwordText.text);

    }

    //call this through the BACK ui button
    public void BackButton()
    {
        logInState = LogInState.usernameEntry;
        passwordFailMessage.SetActive(false);
        usernameFailMessage.SetActive(false);
        waitingForServerResponse = false;
    }

    //call this through the SignUp switch button
    public void SwitchToSignUpMode()
    {
        logInState = LogInState.signup;
        passwordFailMessage.SetActive(false);
        usernameFailMessage.SetActive(false);
        waitingForServerResponse = false;
    }

    //call this through a Register button of some sort
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

    public void TestLoginRPC(NetConnection server)
    {
        Debug.Log("LOGINRPCCALLED");
    }
}
