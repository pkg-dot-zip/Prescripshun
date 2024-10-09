using PrescripshunLib.Networking;
using PrescripshunLib.Networking.Messages;
using Unclassified.Net;

namespace PrescripshunLib.ExtensionMethods;

public static class AsyncTcpClientExtensions
{
    public static async Task Send(this AsyncTcpClient client, IMessage message)
    {
        var bytes = message.Encrypt();

        if (client is null) throw new InvalidOperationException("Client can not be null!!!");
        if (message is null) throw new InvalidOperationException("MESSAGE CAN NOT BE NULL!");

        await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
    }
}