using PrescripshunLib.Networking;
using Unclassified.Net;

namespace PrescripshunLib.ExtensionMethods
{
    public static class AsyncTcpClientExtensions
    {
        public static async Task Send(this AsyncTcpClient client, IMessage message)
        {
            var bytes = message.Encrypt();
            await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        }
    }
}
