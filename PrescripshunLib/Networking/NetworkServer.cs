using System.Net.Sockets;
using System.Net;

namespace PrescripshunLib.Networking
{
    public class NetworkServer : UdpSocket
    {
        public NetworkServer(string address, int port, IReceiveCallback? callback = null)
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            _socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive(callback);
        }
    }
}
