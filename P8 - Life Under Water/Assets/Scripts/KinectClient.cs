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

public class KinectClient : Singleton<KinectClient>
{
    private IPAddress ip;
    public IPAddress Ip { get => ip; set => ip = value; }

    private int port;
    public int Port { get => port; set => port = value; }

    private bool isConnected;
    public bool IsConnected { get => isConnected; }
    private Socket socket;
    private UdpClient udpListener;
    private IPEndPoint endPoint;

    public MeasureDepth measureDepth;

    void Start()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    }

    private void Update()
    {
        if (IsConnected)
        {
            SendKinectDataToServer();
        }
    }

    public void ConnectToServer(IPAddress ip, int port)
    {
        endPoint = new IPEndPoint(ip, port);
        isConnected = true;
    }

    public void SendMessageToServer(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        socket.SendTo(buffer, endPoint);
    }
    public void SendKinectDataToServer()
    {
        string data = JsonUtility.ToJson(measureDepth.kinectData);
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        socket.SendTo(buffer, endPoint);
    }
}
