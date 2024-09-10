using System.Net.Sockets;
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

        public delegate Task OnApplicationBootDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on initiation of the server, before any network code is executed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationBoot { get; set; }


        public delegate Task OnConnectDelegate(TcpClient sender, AsyncTcpClient serverClient, bool isReconnected);

        /// <summary>
        /// Event that gets invoked on the connection of a single client.
        /// </summary>
        public OnConnectDelegate OnConnect { get; set; }

        public delegate void OnReceiveMessageDelegate(TcpClient sender, AsyncTcpClient serverClient, string message);

        /// <summary>
        /// Event that gets invoked upon receiving a message from a client.
        /// </summary>
        public OnReceiveMessageDelegate OnReceiveMessage { get; set; }
        #endregion

    }
}