using System;
using System.Net.Sockets;

public class TCPConnectedClient
{
    #region Data
    private readonly TcpClient connection;
    private readonly byte[] readBuffer = new byte[5000];

    NetworkStream Stream {
        get
        {
            return connection.GetStream();
        }
    }
    #endregion

    #region Init
    public TCPConnectedClient(TcpClient tcpClient)
    {
        this.connection = tcpClient;
        this.connection.NoDelay = true;
        if (TCP.Instance.isServer)
        {
            Stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
        }
    }

    internal void Close()
    {
        connection.Close();
    }
    #endregion

    #region Async Events
    private void OnRead(IAsyncResult ar)
    {
        int length = Stream.EndRead(ar);
        if (length <= 0)
        {
            TCP.Instance.OnDisconnect(this);
        }

        string newMessage = System.Text.Encoding.UTF8.GetString(readBuffer, 0, length);
        TCP.messageToDisplay += newMessage + Environment.NewLine;

        if (TCP.Instance.isServer)
        {
            TCP.BroadcastChatMessage(newMessage);
        }
        Stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    internal void EndConnect(IAsyncResult ar)
    {
        connection.EndConnect(ar);

        Stream.BeginRead(readBuffer, 0, readBuffer.Length, OnRead, null);
    }

    #endregion

    #region API
    internal void Send(string message)
    {
        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(message);
        Stream.Write(buffer, 0, buffer.Length);
    }
    #endregion




}