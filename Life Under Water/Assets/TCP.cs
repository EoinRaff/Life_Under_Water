using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Net.Sockets;
using TMPro;
using System;

public class TCP : Singleton<TCP>
{
    #region Data
    public bool isServer;
    public IPAddress serverIP;
    List<TCPConnectedClient> clientList = new List<TCPConnectedClient>();
    public static string messageToDisplay;
    public TextMeshProUGUI text;
    TcpListener listener;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        base.Awake();
        if (serverIP == null)
        {
            this.isServer = true;
            listener = new TcpListener(localaddr: IPAddress.Any, port: Globals.port);
            listener.Start();
            listener.BeginAcceptTcpClient(OnServerConnect, null);
        }
        else
        {
            TcpClient client = new TcpClient();
            TCPConnectedClient connectedClient = new TCPConnectedClient(client);
            clientList.Add(connectedClient);
            client.BeginConnect(serverIP, Globals.port, (ar) => connectedClient.EndConnect(ar), null); 
        }
    }

    private void OnApplicationQuit()
    {
        listener.Stop();
        for (int i = 0; i < clientList.Count; i++)
        {
            clientList[i].Close();
        }
    }

    void Update()
    {
        text.text = messageToDisplay;
    }
    #endregion

    #region Async Events
    private void OnServerConnect(IAsyncResult ar)
    {
        TcpClient tcpClient = listener.EndAcceptTcpClient(ar);
        clientList.Add(new TCPConnectedClient(tcpClient));
        listener.BeginAcceptTcpClient(OnServerConnect, null);
    }
    #endregion

    #region API
    public void OnDisconnect(TCPConnectedClient client)
    {
        clientList.Remove(client);
    }

    public void Send(string message)
    {
        BroadcastChatMessage(message);
        if (isServer)
        {
            messageToDisplay += message + Environment.NewLine;
        }
    }

    internal static void BroadcastChatMessage(string message)
    {
        for (int i = 0; i < Instance.clientList.Count; i++)
        {
            TCPConnectedClient client = Instance.clientList[i];
            client.Send(message);
        }
    }

    #endregion

}
