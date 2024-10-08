using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;
using PrescripshunLib;
using PrescripshunLib.Networking.Messages;
using Unclassified.Net;

namespace PrescripshunClient
{
    public class ClientEvents
    {
        private static ClientEvents? instance = null;

        private ClientEvents()
        {
        }

        public static ClientEvents Get => instance ??= new ClientEvents();

        public delegate Task OnApplicationBootDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on initiation of the client, before any network code is executed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationBoot { get; set; } = (args) => Task.CompletedTask;


        public delegate Task OnApplicationExitDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on exit of the client, after the connection is closed.
        /// </summary>
        public OnApplicationExitDelegate OnApplicationExit { get; set; } = (args) => Task.CompletedTask;


        public delegate Task OnReceiveStringDelegate(AsyncTcpClient serverClient, [StringSyntax(StringSyntaxAttribute.Json)] string jsonString);

        /// <summary>
        /// Event that gets invoked upon receiving a message from the server.
        /// </summary>
        public OnReceiveStringDelegate OnReceiveJsonString { get; set; } = (client, jsonString) => Task.CompletedTask;

        public delegate Task OnConnectionClosedDelegate(AsyncTcpClient client, bool closedByRemote);
        /// <summary>
        /// Event that gets invoked upon the connection ceasing between the client and the server.
        /// </summary>
        public OnConnectionClosedDelegate OnConnectionClosed { get; set; } = (client, closedByRemote) => Task.CompletedTask;

        public GenericEvent<IMessage> OnReceiveMessage { get; } = new();
    }
}