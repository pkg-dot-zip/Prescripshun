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

        public delegate Task OnApplicationBootDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on initiation of the server, before any network code is executed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationBoot { get; set; }


        public delegate Task OnApplicationExitDelegate(string[] args);

        /// <summary>
        /// Event that gets invoked on exit of the server, after the server is closed.
        /// </summary>
        public OnApplicationBootDelegate OnApplicationExit { get; set; }


        public delegate Task OnConnectDelegate(TcpClient sender, AsyncTcpClient serverClient, bool isReconnected);

        /// <summary>
        /// Event that gets invoked on the connection of a single client.
        /// </summary>
        public OnConnectDelegate OnConnect { get; set; }

        public delegate Task OnReceiveStringDelegate(TcpClient sender, AsyncTcpClient serverClient, string message);

        /// <summary>
        /// Event that gets invoked upon receiving a message from a client.
        /// </summary>
        public OnReceiveStringDelegate OnReceiveString { get; set; }



        public delegate Task OnConnectionClosedDelegate(AsyncTcpClient client, bool closedByRemote);
        /// <summary>
        /// Event that gets invoked upon...
        /// </summary>
        public OnConnectionClosedDelegate OnConnectionClosed { get; set; }
        #endregion



        private readonly Dictionary<Type, Delegate> _messageHandlers = new();
        public delegate Task OnReceiveMessageDelegate<TMessage>(TcpClient sender, AsyncTcpClient serverClient, TMessage message) where TMessage : IMessage;

        // Method to add a handler for a specific message type
        public void AddOnReceiveMessageHandler<TMessage>(OnReceiveMessageDelegate<TMessage> handler) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (_messageHandlers.ContainsKey(messageType))
            {
                _messageHandlers[messageType] = Delegate.Combine(_messageHandlers[messageType], handler);
            }
            else
            {
                _messageHandlers[messageType] = handler;
            }
        }

        // Method to remove a handler for a specific message type
        public void RemoveOnReceiveMessageHandler<TMessage>(OnReceiveMessageDelegate<TMessage> handler) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (_messageHandlers.ContainsKey(messageType))
            {
                _messageHandlers[messageType] = Delegate.Remove(_messageHandlers[messageType], handler);
            }
        }

        // Method to invoke the event based on the message type
        public async Task ReceiveMessage<TMessage>(TcpClient sender, AsyncTcpClient serverClient, TMessage message) where TMessage : IMessage
        {
            var messageType = typeof(TMessage);
            if (_messageHandlers.ContainsKey(messageType))
            {
                var handler = _messageHandlers[messageType] as OnReceiveMessageDelegate<TMessage>;
                if (handler != null)
                {
                    await handler.Invoke(sender, serverClient, message);
                }
            }
        }

    }
}