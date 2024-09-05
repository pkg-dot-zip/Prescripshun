using System.Net.Sockets;
using System.Net;

namespace PrescripshunLib.Networking
{
    public class NetworkClient : UdpSocket
    {
        public NetworkClient(string address, int port, IReceiveCallback? callback = null)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.Connect(IPAddress.Parse(address), port);
            Receive(callback);
        }
    }
}
