using System.Diagnostics;
using NLog;
using Unclassified.Net;

namespace PrescripshunLib;

// NOTE: FIXING THIS HAS A VERY LOW PRIORITY SINCE THIS IS NOT TECHNICALLY NEEDED FOR THIS PROJECT, ONLY IF WE WANT TO REUSE THIS IN FUTURE PROJECTS.
// TODO: Fix. The hardcoded delegate kinda defeats the purpose of this class being generic since the params other than of type T are hardcoded. In other words, don't hardcode AsyncTcpClient as param.

/// <summary>
/// Container for a generic event.
/// </summary>
/// <typeparam name="T">Parameter <see langword="type"/> used in the <see langword="delegate"/>.</typeparam>
public class GenericEvent<T>
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public delegate Task Handler<in TEventType>(AsyncTcpClient serverClient, TEventType message)
        where TEventType : T;

    public interface IEventDelegate
    {
        Task OnEvent(AsyncTcpClient serverClient, T message);
    }

    public class EventDelegate<TEventType> : IEventDelegate where TEventType : T
    {
        public required Handler<TEventType> Implementation { init; get; }

        public Task OnEvent(AsyncTcpClient serverClient, T message)
        {
            if (message is TEventType msg) return Implementation(serverClient, msg);
            return Task.CompletedTask;
        }
    }

    private readonly Dictionary<Type, List<IEventDelegate>> _handlers = [];

    /// <summary>
    /// Adds the <paramref name="handler"/> from this <typeparamref name="TEventType"/>.
    /// </summary>
    /// <typeparam name="TEventType"><see langword="type"/> of <c>T</c> to register <paramref name="handler"/> for.</typeparam>
    /// <param name="handler">Handler to add.</param>
    public void AddHandler<TEventType>(Handler<TEventType> handler) where TEventType : T
    {
        var messageType = typeof(TEventType);
        Logger.Info("Adding handle for {0}", messageType.Name);
        if (!_handlers.ContainsKey(messageType)) _handlers.Add(messageType, []);
        _handlers[messageType].Add(new EventDelegate<TEventType> {Implementation = handler});
    }

    /// <summary>
    /// Removes the <paramref name="handler"/> from this <typeparamref name="TEventType"/>.
    /// </summary>
    /// <typeparam name="TEventType"><see langword="type"/> of <c>T</c> to delete <paramref name="handler"/> for.</typeparam>
    /// <param name="handler">Handler to remove.</param>
    /// <returns><c>true</c> if any handler(s) was removed. <c>false</c> if no handlers were removed.</returns>
    public bool RemoveHandler<TEventType>(Handler<TEventType> handler) where TEventType : T
    {
        var messageType = typeof(TEventType);
        Logger.Info("Removing handle for {0}", messageType.Name);

        if (!_handlers.TryGetValue(messageType, out var value)) return false;

        int nofRemoved = value.RemoveAll(messageHandler =>
            messageHandler is EventDelegate<TEventType> typed && typed.Implementation == handler);

        return nofRemoved != 0;
    }

    /// <summary>
    /// Asynchronously invokes all handlers registered for <typeparamref name="T"/> with <paramref name="message"/>.
    /// We pass an instance of <seealso cref="AsyncTcpClient"/> so that both the server and client can
    /// send an instance of <typeparamref name="T"/> back if needed.
    ///
    /// Please note that <seealso cref="Task.WhenAll(Task[])"/> is used here.
    /// </summary>
    /// <param name="serverClient">Instance of <see cref="AsyncTcpClient"/> so that handlers can send a message of type <typeparamref name="T"/> back.</param>
    /// <param name="message">Instance of <typeparamref name="T"/> to invoke all <see cref="_handlers"/> with.</param>
    /// <returns></returns>
    public async Task Invoke(AsyncTcpClient serverClient, T message)
    {
        Debug.Assert(message != null, nameof(message) + " != null");
        var messageType = message.GetType();

        if (!_handlers.ContainsKey(messageType)) return;

        Logger.Info("Invoking handle for {0}", messageType.Name);
        await Task.WhenAll(_handlers[messageType]
            .Select(handler => handler.OnEvent(serverClient, message))
            .ToArray());
    }
}