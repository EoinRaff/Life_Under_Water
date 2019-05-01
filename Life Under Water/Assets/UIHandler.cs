using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public RectTransform ClientServer;
    public RectTransform TextDisplay;
    public TMP_InputField InputIP;
    public TMP_InputField MessageInput;

    public void BeginProgram()
    {
        ClientServer.gameObject.SetActive(false);
        TextDisplay.gameObject.SetActive(true);
    }

    public void BeginServer()
    {
        Debug.Log("I am Server");
        TCP.Instance.isServer = true;
        BeginProgram();
    }

    public void BeginClient()
    {
        if (System.Enum.TryParse(InputIP.text, out Globals.Server server))
        {
            Debug.Log("Client connecting to " + server.ToString() + " at IP address " + Globals.ServerIP[server].ToString());
            TCP.Instance.serverIP = Globals.ServerIP[server];
            TCP.Instance.isServer = false;
            BeginProgram();
        }
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
        {
            TCP.Instance.Send(MessageInput.text);
            MessageInput.text = "";
        }
    }
}
