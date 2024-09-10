using System.Net.Sockets;
using PrescripshunLib.Networking;
using Unclassified.Net;

namespace PrescripshunServer
{
    internal class ServerEvents
    {
        private static ServerEvents? instance = null;

        private ServerEvents()
        {
        }

        public static ServerEvents Get => instance ??= new ServerEvents();

        #region Events
        public delegate Task OnConnectDelegate(TcpClient connectedClient, AsyncTcpClient serverClient, bool isReconnected);
        public OnConnectDelegate OnConnect { get; set; }

        public delegate void OnReceiveMessageDelegate(TcpClient sender, AsyncTcpClient serverClient, string message);
        public OnReceiveMessageDelegate OnReceiveMessage { get; set; }
        #endregion

    }
}