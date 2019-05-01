using System.Collections.Generic;
using System.Net;

public class Globals
{
    public enum Server { localhost, Sif}
    public static Dictionary<Server, IPAddress> ServerIP = new Dictionary<Server, IPAddress>
    {
        [Server.localhost] = IPAddress.Parse("127.0.0.1"),
        [Server.Sif] = IPAddress.Parse("192.168.1.12")
    };
    public static int port = 56789;
}