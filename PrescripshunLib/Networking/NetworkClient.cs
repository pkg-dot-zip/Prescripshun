using System.Net.Sockets;
using System.Net;

namespace PrescripshunLib.Networking
{
    public class NetworkClient : UdpSocket
    {
        public NetworkClient(string address, int port, IReceiveCallback callback)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Socket.Connect(IPAddress.Parse(address), port);
            Receive(callback);
        }
    }
}
