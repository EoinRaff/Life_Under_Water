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

    private TcpClient socketConnection;
    private Thread clientReceiveThread;
    //private MeasureDepth measureDepth;

    void Start()
    {
        ConnectToTcpServer();
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        //measureDepth = MeasureDepth.Instance;
    }

    private void ConnectToTcpServer()
    {
        try
        {
            clientReceiveThread = new Thread(new ThreadStart(ListenForTcpData));
            clientReceiveThread.IsBackground = true;
            clientReceiveThread.Start();
        }
        catch (Exception)
        {

            throw;
        }
    }

    private void ListenForTcpData()
    {
        try
        {
            endPoint = new IPEndPoint(ip, port);
            socketConnection = new TcpClient(endPoint);
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (NetworkStream stream = socketConnection.GetStream())
                {
                    int length;
                    while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                    {
                        var incomingData = new byte[length];
                        Array.Copy(bytes, 0, incomingData, 0, length);
                        string serverMessage = Encoding.ASCII.GetString(incomingData);
                    }
                }
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
    }

    private void Update()
    {
        /*
        if (IsConnected)
        {
            SendKinectDataUDP();
        }*/
        SendKinectDataTCP();
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
    public void SendKinectDataUDP()
    {
        //string data = JsonUtility.ToJson(measureDepth.kinectData);
        string data = JsonUtility.ToJson(MeasureDepth.Instance.kinectData);
        byte[] buffer = Encoding.ASCII.GetBytes(data);
        socket.SendTo(buffer, endPoint);
    }
    public void SendKinectDataTCP()
    {
        if (socketConnection == null)
            return;
        try
        {
            NetworkStream stream = socketConnection.GetStream();
            if (stream.CanWrite)
            {
                string data = JsonUtility.ToJson(MeasureDepth.Instance.kinectData);
                byte[] buffer = Encoding.ASCII.GetBytes(data);
                stream.Write(buffer, 0, buffer.Length);
            }

        }
        catch (SocketException e)
        {
            Debug.Log(e);
            throw;
        }

        //string data = JsonUtility.ToJson(measureDepth.kinectData);
        
    }
}
