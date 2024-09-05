using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

namespace PrescripshunLib.Networking
{
    public class NetworkClient : UdpSocket
    {
        public NetworkClient(string address, int port)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(IPAddress.Parse(address), port);
            Receive();
        }
    }
}
