using System.Diagnostics;
using System.Net.Sockets;
using NLog;
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
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public delegate Task Handler<in TEventType>(TcpClient sender, AsyncTcpClient serverClient, TEventType message)
            where TEventType : T;

        public interface IEventDelegate
        {
            Task OnEvent(TcpClient sender, AsyncTcpClient serverClient, T message);
        }

        public class EventDelegate<TEventType> : IEventDelegate where TEventType : T
        {
            public required Handler<TEventType> Implementation { init; get; }

            public Task OnEvent(TcpClient sender, AsyncTcpClient serverClient, T message)
            {
                if (message is TEventType msg) return Implementation(sender, serverClient, msg);
                return Task.CompletedTask;
            }
        }

        private readonly Dictionary<Type, List<IEventDelegate>> _handlers = [];

        public void AddHandler<TEventType>(Handler<TEventType> handler) where TEventType : T
        {
            var messageType = typeof(TEventType);
            Logger.Info("Adding handle for {0}", messageType.Name);
            if (!_handlers.ContainsKey(messageType)) _handlers.Add(messageType, []);
            _handlers[messageType].Add(new EventDelegate<TEventType> { Implementation = handler });
        }

        public bool RemoveHandler<TEventType>(Handler<TEventType> handler) where TEventType : T
        {
            var messageType = typeof(TEventType);
            Logger.Info("Removing handle for {0}", messageType.Name);

            if (!_handlers.TryGetValue(messageType, out var value)) return false;

            int nofRemoved = value.RemoveAll(messageHandler =>
                messageHandler is EventDelegate<TEventType> typed && typed.Implementation == handler);

            return nofRemoved != 0;
        }

        public async Task Invoke(TcpClient sender, AsyncTcpClient serverClient, T message)
        {
            Debug.Assert(message != null, nameof(message) + " != null");
            var messageType = message.GetType();

            if (!_handlers.ContainsKey(messageType)) return;

            Logger.Info("Invoking handle for {0}", messageType.Name);
            await Task.WhenAll(_handlers[messageType]
                .Select(handler => handler.OnEvent(sender, serverClient, message))
                .ToArray());
        }
    }
}