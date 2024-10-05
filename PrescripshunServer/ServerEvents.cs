using System.Diagnostics.CodeAnalysis;
using PrescripshunLib;
using PrescripshunLib.Networking.Messages;
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
        public OnApplicationBootDelegate OnApplicationBoot { get; set; } = args => Task.CompletedTask; 


        public delegate Task OnApplicationExitDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on exit of the server, after the server is closed.
        /// </summary>
        public OnApplicationExitDelegate OnApplicationExit { get; set; } = args => Task.CompletedTask;


        public delegate Task OnConnectDelegate(AsyncTcpClient serverClient, bool isReconnected);

        /// <summary>
        /// Event that gets invoked on the connection of a single client.
        /// </summary>
        public OnConnectDelegate OnConnect { get; set; } = (client, isReconnected) => Task.CompletedTask;

        public delegate Task OnReceiveStringDelegate(AsyncTcpClient serverClient, [StringSyntax(StringSyntaxAttribute.Json)] string jsonString);

        /// <summary>
        /// Event that gets invoked upon receiving a message from a client.
        /// </summary>
        public OnReceiveStringDelegate OnReceiveJsonString { get; set; } = (client, jsonString) => Task.CompletedTask;

        public delegate Task OnConnectionClosedDelegate(AsyncTcpClient client, bool closedByRemote);
        /// <summary>
        /// Event that gets invoked upon...
        /// </summary>
        public OnConnectionClosedDelegate OnConnectionClosed { get; set; } = (client, closedByRemote) => Task.CompletedTask;

        public GenericEvent<IMessage> OnReceiveMessage { get; } = new();
        #endregion
    }
}