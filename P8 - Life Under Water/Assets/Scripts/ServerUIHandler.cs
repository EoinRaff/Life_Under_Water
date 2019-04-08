using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ServerUIHandler : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Update()
    {
        if (KinectServer.instance.Message == "")
        {
            text.text = "Awaiting Message from Client";
        }
        else
        {
            text.text = "Recieved Message: \n" + KinectServer.instance.Message;
        }
    }
}
