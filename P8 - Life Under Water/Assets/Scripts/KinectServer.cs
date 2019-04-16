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
    #region Connection Settings 
    [SerializeField]
    private int port = 11000;
    public int Port { get => port; }

    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private UdpClient listener;
    private IPEndPoint groupEP;

    Thread receiveThread;

    #endregion

    #region Data Processing
    string recievedMessage;
    public string Message { get => recievedMessage; set => recievedMessage = value; }
    
    public KinectData Data { get; set; }
    public Vector2 CenterOfMass { get; set; }
    public List<Vector2> TriggerPoints { get; set; }
    #endregion

    void Start()
    {
        recievedMessage = "";
        receiveThread = new Thread(new ThreadStart(ReceiveData)){IsBackground = true};
        receiveThread.Start();
    }

    private void ReceiveData()
    {
        listener = new UdpClient(port);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = listener.Receive(ref anyIP);

                string json = Encoding.UTF8.GetString(data);

                recievedMessage = json;
                Data = JsonToKinectData(json);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
        }
    }

    private KinectData JsonToKinectData(string message)
    {
        return JsonUtility.FromJson<KinectData>(message);
    }

    public void GetTriggerPointsFromKinectData(KinectData kinectData)
    {
        List<Vector2> triggerPoints = new List<Vector2>();
        for (int i = 0; i < kinectData.triggerPoints.Length; i++)
        {
            triggerPoints.Add(kinectData.triggerPoints[i]);
        }
        TriggerPoints = triggerPoints;
    }

}