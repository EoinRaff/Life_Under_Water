using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class KinectServer : MonoBehaviour
{
    public static KinectServer instance;

    [SerializeField]
    private int port;
    public int Port { get => port; }

    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private List<IPEndPoint> clients = new List<IPEndPoint>();
    private UdpClient listener;
    private IPEndPoint groupEP;

    private void Awake()
    {
        Debug.Log("Creating UDP Listener");
        listener = new UdpClient(port);
    }
    void Start()
    {
        Singleton();
        groupEP = new IPEndPoint(IPAddress.Any, port);
    }

    void Update()
    {
        try
        {
            Debug.Log("Waiting for broadcast");
            byte[] bytes = listener.Receive(ref groupEP);

            Debug.Log($"Received broadcast from {groupEP} :");
            Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
        finally
        {
            listener.Close();
        }
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

    public void ListenForClients()
    {
        try
        {
            while (true)
            {
                Debug.Log("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP);

                Debug.Log($"Received broadcast from {groupEP} :");
                Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
            }
        }
        catch (SocketException e)
        {
            Debug.Log(e);
        }
        finally
        {
            listener.Close();
        }
    }

    public void SendMessageToClient(string message)
    {
        throw new NotImplementedException();
    }
}