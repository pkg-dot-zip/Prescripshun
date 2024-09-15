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

        public delegate Task Handler<in EventType>(TcpClient sender, AsyncTcpClient serverClient, EventType message) where EventType : T;

        public interface IEventDelegate
        {
            Task OnEvent(TcpClient sender, AsyncTcpClient serverClient, T message);
        }

        public class EventDelegate<EventType> : IEventDelegate where EventType : T
        {
            public required Handler<EventType> Implementation { init; get; }

            public Task OnEvent(TcpClient sender, AsyncTcpClient serverClient, T message)
            {
                if (message is EventType msg)
                {
                    return Implementation(sender, serverClient, msg);
                }

                return Task.CompletedTask;
            }
        }

        private readonly Dictionary<Type, List<IEventDelegate>> _handlers = [];

        public void AddHandler<EventType>(Handler<EventType> handler) where EventType : T
        {
            var messageType = typeof(EventType);
            Logger.Info("Adding handle for {0}", messageType.Name);

            if (!_handlers.ContainsKey(messageType))
            {
                _handlers.Add(messageType, []);
            }

            _handlers[messageType].Add(new EventDelegate<EventType> { Implementation = handler });
        }

        public bool RemoveHandler<EventType>(Handler<EventType> handler) where EventType : T
        {
            var messageType = typeof(EventType);
            Logger.Info("Removing handle for {0}", messageType.Name);

            if (!_handlers.TryGetValue(messageType, out List<GenericEvent<T>.IEventDelegate>? value)) return false;

            int nofRemoved = value.RemoveAll(messageHandler => {
                return messageHandler is EventDelegate<EventType> typed && typed.Implementation == handler;
            });

            return nofRemoved != 0;
        }

        public async Task Invoke(TcpClient sender, AsyncTcpClient serverClient, T message)
        {
            Debug.Assert(message != null, nameof(message) + " != null");
            var messageType = message.GetType();
            Logger.Info("Attempting Invoke handle for {0}", messageType.Name);

            if (!_handlers.ContainsKey(messageType)) return;

            await Task.WhenAll(_handlers[messageType]
                .Select(handler => handler.OnEvent(sender, serverClient, message))
                .ToArray());
        }
    }
}