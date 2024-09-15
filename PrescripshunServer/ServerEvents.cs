using System.Net.Sockets;
using PrescripshunLib;
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

        public delegate Task OnApplicationBootDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on initiation of the server, before any network code is executed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationBoot { get; set; }


        public delegate Task OnApplicationExitDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on exit of the server, after the server is closed.
        /// </summary>
        public OnApplicationExitDelegate OnApplicationExit { get; set; }


        public delegate Task OnConnectDelegate(TcpClient sender, AsyncTcpClient serverClient, bool isReconnected);

        /// <summary>
        /// Event that gets invoked on the connection of a single client.
        /// </summary>
        public OnConnectDelegate OnConnect { get; set; }

        public delegate Task OnReceiveStringDelegate(TcpClient sender, AsyncTcpClient serverClient, string jsonString);

        /// <summary>
        /// Event that gets invoked upon receiving a message from a client.
        /// </summary>
        public OnReceiveStringDelegate OnReceiveJsonString { get; set; }



        public delegate Task OnConnectionClosedDelegate(AsyncTcpClient client, bool closedByRemote);
        /// <summary>
        /// Event that gets invoked upon...
        /// </summary>
        public OnConnectionClosedDelegate OnConnectionClosed { get; set; }

        public GenericEvent<IMessage> OnReceiveMessage { get; } = new();
        #endregion
    }
}