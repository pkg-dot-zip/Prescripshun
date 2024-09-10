using PrescripshunLib.Networking;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unclassified.Net;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient
{
    private static void Main(string[] args)
    {
        var server = new Server();
        server.RegisterEvents();
        RunAsync().GetAwaiter().GetResult();
    }

    private static async Task RunAsync()
    {
        var server = new AsyncTcpListener
        {
            IPAddress = NetworkHandler.AnyIpAddress,
            Port = NetworkHandler.Port,

            ClientConnectedCallback = tcpClient =>
                new AsyncTcpClient
                {
                    ServerTcpClient = tcpClient,

                    ConnectedCallback = async (serverClient, isReconnected) => ServerEvents.Get.OnConnect.Invoke(tcpClient, serverClient, isReconnected),

                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Console.WriteLine("Server client: received: " + message + $" | FROM: {serverClient.ServerTcpClient.Client.RemoteEndPoint}");

                        bytes = Encoding.UTF8.GetBytes("You said: " + message);
                        await serverClient.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));

                        if (message == "bye") serverClient.Disconnect(); // Let the server close the connection

                        ServerEvents.Get.OnReceiveMessage.Invoke(tcpClient, serverClient, message);
                    }
                }.RunAsync()
        };
        server.Message += (s, a) => Console.WriteLine("Server: " + a.Message);
        var serverTask = server.RunAsync();

        await serverTask;
    }

    public void RegisterEvents()
    {
        ServerEvents.Get.OnConnect += async (client, server, reconnected) =>
        {
            await Task.Delay(500);
            byte[] bytes = Encoding.UTF8.GetBytes($"Hello, {client.Client.RemoteEndPoint}, my name is Server. Talk to me.");
            await server.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        };

        ServerEvents.Get.OnReceiveMessage += (sender, server, message) =>
        {
            Console.WriteLine($"Printing from OnReceive: \"{message}\" - {sender.Client.RemoteEndPoint}");
        };
    }
}