using PrescripshunLib.Networking;
using System.Net;
using System.Text;
using Unclassified.Net;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient
{
    private static void Main(string[] args)
    {
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        int port = NetworkHandler.Port;

        var server = new AsyncTcpListener
        {
            IPAddress = IPAddress.IPv6Any,
            Port = port,
            ClientConnectedCallback = tcpClient =>
                new AsyncTcpClient
                {
                    ServerTcpClient = tcpClient,
                    ConnectedCallback = async (serverClient, isReconnected) =>
                    {
                        await Task.Delay(500);
                        byte[] bytes = Encoding.UTF8.GetBytes($"Hello, {tcpClient.Client.RemoteEndPoint}, my name is Server. Talk to me.");
                        await serverClient.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
                    },
                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Console.WriteLine("Server client: received: " + message);

                        bytes = Encoding.UTF8.GetBytes("You said: " + message);
                        await serverClient.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));

                        if (message == "bye")
                        {
                            // Let the server close the connection
                            serverClient.Disconnect();
                        }
                    }
                }.RunAsync()
        };
        server.Message += (s, a) => Console.WriteLine("Server: " + a.Message);
        var serverTask = server.RunAsync();

        await serverTask;
    }
}