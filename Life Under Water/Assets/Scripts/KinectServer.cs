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
    public static IPAddress localIP = IPAddress.Parse("192.168.1.18");
    public static int port = 11000;
    private const int NUMBER_OF_KINECTS = 6;

    #region Connection Settings 
    [SerializeField]
    private int[] ports = new int[NUMBER_OF_KINECTS];
    private Thread[] listenThreads = new Thread[NUMBER_OF_KINECTS];


//    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private UdpClient[] listeners = new UdpClient[NUMBER_OF_KINECTS];
    private TcpListener listener;
    private IPEndPoint groupEP;
    private Socket socket;
    private Thread TcpListenThread;
    private TcpClient connectedTcpClient;

    #endregion

    #region Data Processing
    string recievedMessage;
    public string Message { get; set; }

    private KinectData recievedData;
    public KinectData Data { get; set; }
    //public List<KinectData> kinects;
    private KinectData[] kinects;
    public Vector2 CenterOfMass { get; private set; }
    public List<Vector2> TriggerPoints { get; set; }
    #endregion

    void Start()
    {
        Message = "";
        recievedMessage = "";
        kinects = new KinectData[NUMBER_OF_KINECTS];
        /* for (int i = 0; i < NUMBER_OF_KINECTS; i++)
         {
             listenThreads[i] = new Thread(new ParameterizedThreadStart(ReceiveData))
             {
                 IsBackground = true
             };
             listenThreads[i].Start(i);

         }*/
        TcpListenThread = new Thread(new ThreadStart(ListenForTCPClient));
        TcpListenThread.IsBackground = true;
        TcpListenThread.Start();
    }

    private void ListenForTCPClient()
    {
        try
        {
            listener = new TcpListener(localIP, port);
            listener.Start();
            byte[] bytes = new byte[1024];
            while (true)
            {
                using (connectedTcpClient = listener.AcceptTcpClient())
                {
                    using (NetworkStream stream = connectedTcpClient.GetStream())
                    {
                        int length;
                        while ((length = stream.Read(bytes, 0, bytes.Length)) != 0)
                        {
                            var incomingData = new byte[length];
                            Array.Copy(bytes, 0, incomingData, 0, length);
                            string clientMessage = Encoding.ASCII.GetString(incomingData);
                            Message = clientMessage;
                        }
                    }
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
    }

    private void Update()
    {
        Data = CombineClientData(out List<Vector2> tempList, out string msg);
        if (Data == null)
            return;

        TriggerPoints = tempList;
        msg += "Combined center of mass: " + Data.centerOfMass;
        Message = msg;
    }

    private void OnApplicationQuit()
    {
        foreach (Thread thread in listenThreads)
        {
            thread.Abort();
        }
    }

    private void ReceiveUdpData(object index)
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

                Debug.Log(string.Format("IP #{0}: {1}", i + 1, anyIP.Address));


                string json = Encoding.UTF8.GetString(data);

                recievedMessage = json;
                recievedData = JsonToKinectData(json);
                print(string.Format("recieved Data from IP {0} at port {1}", anyIP.Address, ports[i]));

                kinects[i] = recievedData;
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
        if (kinectData.triggerPoints == null)
            return new List<Vector2>();
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
            if (item == null || item.kinectID == 0)
                continue;
            newData.centerOfMass = newData.centerOfMass + item.centerOfMass;
            triggerPoints.AddRange(GetTriggerPointsFromKinectData(item));
            msg += string.Format("Kinect #{0} offset center of mass: {1}\n", item.kinectID, item.centerOfMass+item.offset);
        }
//        newData.centerOfMass /= kinects.Count;
        TriggerPoints = triggerPoints;
        return newData;
    }

}