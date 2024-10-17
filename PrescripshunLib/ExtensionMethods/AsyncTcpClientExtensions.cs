using PrescripshunLib.Networking;
using PrescripshunLib.Networking.Messages;
using Unclassified.Net;

namespace PrescripshunLib.ExtensionMethods;

/// <summary>
/// Contains extension methods for <seealso cref="AsyncTcpClient"/>.
/// </summary>
public static class AsyncTcpClientExtensions
{
    /// <summary>
    /// Asynchronously calls <see cref="Encryptor.Encrypt(IMessage)"/> on this <paramref name="message"/> to convert it to a <c>byte[]</c>, then immediately sends it by
    /// calling <see cref="AsyncTcpClient.Send"/>.
    /// </summary>
    /// <param name="client">Client to send with.</param>
    /// <param name="message">Message to send.</param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static async Task Send(this AsyncTcpClient client, IMessage message)
    {
        var bytes = message.Encrypt();

        if (client is null) throw new InvalidOperationException("Client can not be null!!!");
        if (message is null) throw new InvalidOperationException("MESSAGE CAN NOT BE NULL!");

        await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
    }
}