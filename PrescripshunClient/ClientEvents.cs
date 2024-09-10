using System.Net.Sockets;
using PrescripshunLib.Networking;
using Unclassified.Net;

namespace PrescripshunClient
{
    internal class ClientEvents
    {
        private static ClientEvents? instance = null;

        private ClientEvents()
        {
        }

        public static ClientEvents Get => instance ??= new ClientEvents();


        #region Events
        public delegate Task OnApplicationBootDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on initiation of the client, before any network code is executed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationBoot { get; set; }


        public delegate Task OnApplicationExitDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on exit of the client, after the connection is closed.
        /// </summary>
        public OnApplicationExitDelegate OnApplicationExit { get; set; }


        public delegate Task OnReceiveDelegate(AsyncTcpClient client, string text);
        /// <summary>
        /// Event that gets invoked upon receiving a message from the server.
        /// </summary>
        public OnReceiveDelegate OnReceive { get; set; }

        #endregion
    }
}