using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using TMPro;

public class ClientUIHandler : MonoBehaviour
{
    public TextMeshProUGUI statusText;

    public TMP_InputField ipInput;
    public TMP_InputField portInput;
    public TMP_InputField messageInput;

    private IPAddress ip;
    private int port;

    void Awake()
    {
        statusText.text = "";
        //ipInput.characterValidation = TMP_InputField.CharacterValidation.Decimal;
        //portInput.characterValidation = TMP_InputField.CharacterValidation.Integer;
    }

    public bool TryParseInputValues()
    {
        // Validate IP Address
        if (!IPAddress.TryParse(ipInput.text, out ip))
        {
            statusText.text = "IP Address not valid.";
            return false;
        }
        // Validate Port as Integer
        if (!int.TryParse(portInput.text, out port))
        {
            statusText.text = "Port not valid.";
            return false;
        }

        // Validate Port is in correct range
        if (port < 1024 || port > 65535)
        {
            statusText.text = "Port not valid. \nPlease choose port between 1024 and 65535.";
            return false;
        }

        statusText.text = "IP Address & Port Accepted.";
        return true;
    }

    public void ConnectToServer()
    {
        if (!TryParseInputValues())
            return;

        KinectClient.instance.ConnectToServer(ip, port);
        statusText.text = string.Format("Connected to {0} at port {1}", ip, port);
    }

    public void SendMessage()
    {
        if (!KinectClient.instance.IsConnected)
        {
            statusText.text = "Cannot send message. \nClient not connect to Server.";
            return;
        }

        //for debugging only
        Debug.Log(messageInput.text);
        statusText.text = "Message sent to Server";
        KinectClient.instance.SendMessageToServer(messageInput.text);
    }


}
