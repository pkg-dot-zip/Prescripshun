using System.Net.Sockets;
using Unclassified.Net;

namespace PrescripshunLib
{
    public class GenericEvent<T>
    {
        private readonly Dictionary<Type, Delegate> _messageHandlers = new();
        public delegate Task OnReceiveMessageDelegate<ImplementationType>(TcpClient sender, AsyncTcpClient serverClient, ImplementationType message) where ImplementationType : T;

        // Method to add a handler for a specific message type
        public void AddHandler<ImplementationType>(OnReceiveMessageDelegate<ImplementationType> handler) where ImplementationType : T
        {
            var messageType = typeof(ImplementationType);
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
        public void RemoveHandler<ImplementationType>(OnReceiveMessageDelegate<ImplementationType> handler) where ImplementationType : T
        {
            var messageType = typeof(ImplementationType);
            if (_messageHandlers.ContainsKey(messageType))
            {
                _messageHandlers[messageType] = Delegate.Remove(_messageHandlers[messageType], handler);
            }
        }

        // Method to invoke the event based on the message type
        public async Task Invoke<ImplementationType>(TcpClient sender, AsyncTcpClient serverClient, ImplementationType message) where ImplementationType : T
        {
            var messageType = typeof(ImplementationType);
            if (_messageHandlers.ContainsKey(messageType))
            {
                var handler = _messageHandlers[messageType] as OnReceiveMessageDelegate<ImplementationType>;
                if (handler != null)
                {
                    await handler.Invoke(sender, serverClient, message);
                }
            }
        }
    }
}
