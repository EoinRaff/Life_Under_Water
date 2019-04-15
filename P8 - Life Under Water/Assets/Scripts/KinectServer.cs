using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

public class KinectServer : Singleton<KinectServer>
{
    [SerializeField]
    private int port = 11000;
    public int Port { get => port; }

    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private UdpClient listener;
    private IPEndPoint groupEP;

    Thread receiveThread;
    string recievedMessage;
    public string Message { get => recievedMessage; set => recievedMessage = value; }


    void Start()
    {
        recievedMessage = "";
        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.IsBackground = true;
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        Debug.Log("Creating UDP Listener");
        listener = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = listener.Receive(ref anyIP);

                string text = Encoding.UTF8.GetString(data);

                print(">> " + text);

                recievedMessage = text;
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    void Update()
    {
        Debug.Log("Waiting for broadcast");
        //byte[] bytes = listener.Receive(ref groupEP);

        Debug.Log(recievedMessage);
        //Debug.Log($" {Encoding.ASCII.GetString(bytes, 0, bytes.Length)}");
    }

    public IEnumerator ListenForClients()
    {
        try
        {
            while (true)
            {
                Debug.Log("Waiting for broadcast");
                byte[] bytes = listener.Receive(ref groupEP); // I think this line causes the crashes

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
        yield return new WaitForEndOfFrame();
    }

    public void SendMessageToClient(string message)
    {
        throw new NotImplementedException();
    }
}