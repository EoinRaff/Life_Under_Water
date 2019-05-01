using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;

public class BasicServer : MonoBehaviour
{
    public enum SmileComputers { Sif, Thor, Odin, Panorama }
    private Dictionary<SmileComputers, IPAddress> smileComputerIPAddresses = new Dictionary<SmileComputers, IPAddress>
    {
        [SmileComputers.Sif] = IPAddress.Parse("192.168.1.12"),
        //[SmileComputers.Thor] = IPAddress.Parse(""),
        //[SmileComputers.Odin] = IPAddress.Parse(""),
        //[SmileComputers.Panorama] = IPAddress.Parse("")
    };
    [SerializeField]
    private int listenPort = 8888;
    private Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
    private IPEndPoint endPoint;
    private UdpClient udpClient;
    private Thread listenThread;
    
    public string Message { get; private set; }

    void Start()
    {
        listenThread = new Thread(new ThreadStart(ListenForClient))
        {
            IsBackground = true
        };
        listenThread.Start();

    }

    private void ListenForClient()
    {
        udpClient = new UdpClient(listenPort);
        while (true)
        {
            try
            {
                IPEndPoint anyIP = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = udpClient.Receive(ref anyIP);

                Message = Encoding.UTF8.GetString(data);
            }
            catch (Exception err)
            {
                print(err.ToString());
            }
            
            Thread.Sleep(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Message: " + Message);
    }
}
