using PrescripshunLib.Networking;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Unclassified.Net;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient, IReceiveCallback
{
    private static void Main(string[] args)
    {
        var server = new Server();
        server.RegisterEvents();
        RunAsync(server).GetAwaiter().GetResult();
    }

    private static async Task RunAsync(Server receiveCallback)
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

                    ConnectedCallback = async (serverClient, isReconnected) => ServerEvents.Get.OnConnect.Invoke(tcpClient, serverClient, isReconnected),

                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Console.WriteLine("Server client: received: " + message + $" | FROM: {serverClient.ServerTcpClient.Client.RemoteEndPoint}");

                        bytes = Encoding.UTF8.GetBytes("You said: " + message);
                        await serverClient.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));

                        if (message == "bye") serverClient.Disconnect(); // Let the server close the connection

                        receiveCallback.OnReceive(new ReceivedArgs(serverClient.ServerTcpClient.Client.RemoteEndPoint!, message));
                    }
                }.RunAsync()
        };
        server.Message += (s, a) => Console.WriteLine("Server: " + a.Message);
        var serverTask = server.RunAsync();

        await serverTask;
    }


    public void OnReceive(ReceivedArgs args) => ServerEvents.Get.OnReceiveMessage.Invoke(args);

    public void RegisterEvents()
    {
        ServerEvents.Get.OnConnect += async (client, server, reconnected) =>
        {
            await Task.Delay(500);
            byte[] bytes = Encoding.UTF8.GetBytes($"Hello, {client.Client.RemoteEndPoint}, my name is Server. Talk to me.");
            await server.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        };

        ServerEvents.Get.OnReceiveMessage += args =>
        {
            var sender = args.EndPointReceivedFrom;
            var message = args.Text;

            Console.WriteLine($"Printing from OnReceive: \"{message}\" - {sender}");
        };
    }
}