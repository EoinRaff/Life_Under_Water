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
    private const int NUMBER_OF_KINECTS = 2;
    #region Connection Settings 
    [SerializeField]
    public int[] ports = new int[NUMBER_OF_KINECTS];
    private Thread[] listenThreads = new Thread[NUMBER_OF_KINECTS];
    //private int port = 11000;
    //public int Port { get => port; }

    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private UdpClient[] listeners = new UdpClient[NUMBER_OF_KINECTS];
    private IPEndPoint groupEP;

    #endregion

    #region Data Processing
    string recievedMessage;
    public string Message { get => recievedMessage; set => recievedMessage = value; }

    private KinectData recievedData;
    public KinectData Data { get; set; }
    //public List<KinectData> kinects;
    public KinectData[] kinects;
    public Vector2 CenterOfMass { get; private set; }
    public List<Vector2> TriggerPoints { get; set; }
    #endregion

    void Start()
    {
        recievedMessage = "";
        kinects = new KinectData[NUMBER_OF_KINECTS];
        for (int i = 0; i < NUMBER_OF_KINECTS; i++)
        {
            listenThreads[i] = new Thread(new ParameterizedThreadStart(ReceiveData))
            {
                IsBackground = true
            };
            listenThreads[i].Start(i);

        }
    }

    private void Update()
    {
        Data = CombineClientData(out List<Vector2> tempList, out string msg);
        if (Data == null)
            return;

        TriggerPoints = tempList;
        msg += "Combined center of mass: " + Data.centerOfMass;
        Debug.Log(msg);
    }

    private void OnApplicationQuit()
    {
        foreach (Thread thread in listenThreads)
        {
            thread.Abort();
        }
    }

    private void ReceiveData(object index)
    {
        int i = Convert.ToInt32(index);
        listeners[i] = new UdpClient(ports[i]);
        print("Listening at Port " + ports[i]);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = listeners[i].Receive(ref anyIP);

                string json = Encoding.UTF8.GetString(data);

                recievedMessage = json;
                recievedData = JsonToKinectData(json);
                print(string.Format("recieved Data from Kinect #{0} at port {1}", recievedData.kinectID, ports[i]));

                kinects[i] = recievedData;
                /*
                if (!ListContainsClient(kinects, recievedData))
                {
                    kinects.Add(recievedData);
                }*/
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
            Thread.Sleep(0);
        }
    }

    private KinectData JsonToKinectData(string message)
    {
        return JsonUtility.FromJson<KinectData>(message);
    }

    private List<Vector2> GetTriggerPointsFromKinectData(KinectData kinectData)
    {
        List<Vector2> triggerPoints = new List<Vector2>();
        for (int i = 0; i < kinectData.triggerPoints.Length; i++)
        {
            triggerPoints.Add(kinectData.triggerPoints[i] + kinectData.offset); 
        }
        return triggerPoints;
    }

    private bool ListContainsClient(List<KinectData> list, KinectData client)
    {
        foreach (KinectData item in list)
        {
            if (item.kinectID == client.kinectID)
                return true;
        }
        return false;
    }

    private KinectData CombineClientData(out List<Vector2> TriggerPoints, out string msg)
    {
        msg = "";
        if (recievedData == null)
        {
            TriggerPoints = null;
            msg = "";
            return null;
        }
        int arraySize = recievedData.triggerPoints.Length;
        KinectData newData = new KinectData(arraySize){centerOfMass = Vector2.zero};
        List<Vector2> triggerPoints = new List<Vector2>();
        foreach (KinectData item in kinects)
        {
            newData.centerOfMass =  newData.centerOfMass + item.centerOfMass;
            triggerPoints.AddRange(GetTriggerPointsFromKinectData(item));
            msg += string.Format("Kinect #{0} offset center of mass: {1}\n", item.kinectID, item.centerOfMass+item.offset);
        }
//        newData.centerOfMass /= kinects.Count;
        TriggerPoints = triggerPoints;
        return newData;
    }

}