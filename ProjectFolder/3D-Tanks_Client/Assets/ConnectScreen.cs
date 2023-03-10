using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConnectScreen : MonoBehaviour
{
    public InputField address;
    public InputField port;

    // Start is called before the first frame update
    void Start()
    {
        ConnectButton();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ConnectButton()
    {
        ClientGameLogic.instance.ConnectToServer(address.text, System.Int32.Parse(port.text));
    }
}
