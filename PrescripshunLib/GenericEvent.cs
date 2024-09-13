using System.Diagnostics;
using System.Net.Sockets;
using NLog;
using PrescripshunLib.Networking;
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
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private readonly Dictionary<string, Delegate> _handlers = new();
        public delegate Task OnReceiveMessageDelegate<in TImplementationType>(TcpClient sender, AsyncTcpClient serverClient, TImplementationType message) where TImplementationType : T;

        /// <summary>
        /// Adds a <see cref="handler"/>> for a specific message <see langword="type"/> <see cref="TImplementationType"/>.
        /// </summary>
        /// <typeparam name="TImplementationType"><see langword="type"/> of message</typeparam>
        /// <param name="handler">Implementation for handling <see langword="type"/> <see cref="TImplementationType"/>.</param>
        public void AddHandler<TImplementationType>(OnReceiveMessageDelegate<TImplementationType> handler) where TImplementationType : T
        {
            var messageType = typeof(TImplementationType);
            Logger.Info("Adding handle for {0}", messageType.Name);
            if (!_handlers.TryAdd(messageType.Name, handler))
            {
                _handlers[messageType.Name] = Delegate.Combine(_handlers[messageType.Name], handler);
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
            Logger.Info("Removing handle for {0}", messageType.Name);
            if (_handlers.ContainsKey(messageType.Name))
            {
                try
                {
                    _handlers[messageType.Name] = Delegate.Remove(_handlers[messageType.Name], handler) ?? throw new InvalidOperationException();
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
            Debug.Assert(message != null, nameof(message) + " != null");
            Logger.Info("Attempting Invoke handle for {0}", message.GetType().Name);
            if (_handlers.TryGetValue(message.GetType().Name, out var messageHandler))
            {
                if (messageHandler is OnReceiveMessageDelegate<TImplementationType> handler)
                {
                    await handler.Invoke(sender, serverClient, message);
                }
                else
                {
                    Logger.Info("Couldn't invoke handle for {0}", message.GetType().Name);
                    messageHandler.DynamicInvoke(sender, serverClient, Convert.ChangeType(message, message.GetType()));
                }
            }
            else
            {
                throw new ArgumentException($"Couldn't Invoke for type: {typeof(TImplementationType)}");
            }
        }
    }
}
