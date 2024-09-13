using System.Net.Sockets;
using Unclassified.Net;

namespace PrescripshunLib
{

    // NOTE: FIXING THIS HAS A VERY LOW PRIORITY SINCE THIS IS NOT TECHNICALLY NEEDED FOR THIS PROJECT, ONLY IF WE WANT TO REUSE THIS IN FUTURE PROJECTS.
    // TODO: Fix. The hardcoded delegate kinda defeats the purpose of this class being generic since the params other than of type T are hardcoded.
    // TODO: After fixing the above problem, fix the docs so it fits the generic state of this class.

    /// <summary>
    /// Container for a generic event.
    /// </summary>
    /// <typeparam name="T">Parameter <see langword="type"/> used in the <see langword="delegate"/>.</typeparam>
    public class GenericEvent<T>
    {
        private readonly Dictionary<Type, Delegate> _handlers = new();
        public delegate Task OnReceiveMessageDelegate<in TImplementationType>(TcpClient sender, AsyncTcpClient serverClient, TImplementationType message) where TImplementationType : T;

        /// <summary>
        /// Adds a <see cref="handler"/>> for a specific message <see langword="type"/> <see cref="TImplementationType"/>.
        /// </summary>
        /// <typeparam name="TImplementationType"><see langword="type"/> of message</typeparam>
        /// <param name="handler">Implementation for handling <see langword="type"/> <see cref="TImplementationType"/>.</param>
        public void AddHandler<TImplementationType>(OnReceiveMessageDelegate<TImplementationType> handler) where TImplementationType : T
        {
            var messageType = typeof(TImplementationType);
            if (!_handlers.TryAdd(messageType, handler))
            {
                _handlers[messageType] = Delegate.Combine(_handlers[messageType], handler);
            }
        }


        /// <summary>
        /// Removes a <see cref="handler"/> for a specific message <see langword="type"/> <see cref="TImplementationType"/>. Returns <see langword="true"/> if successful.
        /// </summary>
        /// <typeparam name="TImplementationType">><see langword="type"/> of message.</typeparam>
        /// <param name="handler">Implementation for handling <see langword="type"/> <see cref="TImplementationType"/>.</param>
        /// <returns><see langword="true"/> if successful. <see langword="false"/> if <see cref="handler"/> could not be found.</returns>
        public bool RemoveHandler<TImplementationType>(OnReceiveMessageDelegate<TImplementationType> handler) where TImplementationType : T
        {
            var messageType = typeof(TImplementationType);
            if (_handlers.ContainsKey(messageType))
            {
                try
                {
                    _handlers[messageType] = Delegate.Remove(_handlers[messageType], handler) ?? throw new InvalidOperationException();
                }
                catch
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Method to invoke the event based on the message <see langword="type"/> <see cref="TImplementationType"/>.
        /// </summary>
        /// <typeparam name="TImplementationType"><see langword="type"/> of message</typeparam>
        /// <param name="sender">TcpClient</param>
        /// <param name="serverClient">AsyncTcpClient</param>
        /// <param name="message">Message that will be passed as an argument to all subscribers of that <see langword="type"/> of <see cref="message"/>>.</param>
        /// <returns></returns>
        public async Task Invoke<TImplementationType>(TcpClient sender, AsyncTcpClient serverClient, TImplementationType message) where TImplementationType : T
        {
            if (_handlers.TryGetValue(typeof(TImplementationType), out var messageHandler))
            {
                if (messageHandler is OnReceiveMessageDelegate<TImplementationType> handler)
                {
                    await handler.Invoke(sender, serverClient, message);
                }
            }
        }
    }
}
