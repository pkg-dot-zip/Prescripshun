using System.Net.Sockets;
using System.Net;

namespace PrescripshunLib.Networking
{
    public class NetworkServer : UdpSocket
    {
        public NetworkServer(string address, int port, IReceiveCallback callback)
        {
            Socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            Socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            Socket.Bind(new IPEndPoint(IPAddress.Parse(address), port));
            Receive(callback);
        }
    }
}
