using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class KinectClient : MonoBehaviour
{
    public static KinectClient instance;

    private IPAddress ip;
    public IPAddress Ip { get => ip; set => ip = value; }

    private int port;
    public int Port { get => port; set => port = value; }

    private bool isConnected;
    public bool IsConnected { get => isConnected; }

    private Socket socket;
    private UdpClient udpListener;
    private IPEndPoint endPoint;

    void Start()
    {
        Singleton();
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    void Update()
    {

    }

    private void Singleton()
    {
        /* Using a Singleton pattern ensures that there is only ever one client running per kinect, 
         * and allow references to a static instance of the class */

        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
    }

    public void ConnectToServer(IPAddress ip, int port)
    {
        endPoint = new IPEndPoint(ip, port);
        isConnected = true;
    }

    public void SendMessageToServer(string message)
    {
        print("encoding message");
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        print("Sending message to endpoint");
        socket.SendTo(buffer, endPoint);
    }
}
