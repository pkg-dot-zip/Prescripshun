using System.Net.Sockets;
using PrescripshunLib.Networking;
using System.Text;
using PrescripshunLib.Logging;
using Unclassified.Net;
using System.Diagnostics.CodeAnalysis;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public bool IsRunning = true;

    private static void Main(string[] args)
    {
        LogHandler.Configure("server");

        // First we create an instance of the server and register all the events.
        var server = new Server();
        server.RegisterEvents();

        // Then we run the logic.
        ServerEvents.Get.OnApplicationBoot.Invoke(args);
        server.RunServer();

        while (server.IsRunning)
        {
            string inputLine = Console.ReadLine() ?? string.Empty;

            if (inputLine == string.Empty) continue;
            if (inputLine == "quit") server.IsRunning = false;
        }

        ServerEvents.Get.OnApplicationExit.Invoke(args);
    }

    private async Task RunServer()
    {
        var server = new AsyncTcpListener
        {
            IPAddress = NetworkHandler.AnyIpAddress,
            Port = NetworkHandler.Port,

            ClientConnectedCallback = tcpClient => // tcpClient = acceptedConnection.
                new AsyncTcpClient
                {
                    ServerTcpClient = tcpClient,

                    ConnectedCallback = async (serverClient, isReconnected) =>
                        ServerEvents.Get.OnConnect.Invoke(tcpClient, serverClient, isReconnected),

                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Logger.Info("Server: received: " + message +
                                    $" | FROM: {serverClient.ServerTcpClient.Client.RemoteEndPoint}");

                        if (message == "bye") serverClient.Disconnect(); // Let the server close the connection.

                        ServerEvents.Get.OnReceiveJsonString.Invoke(tcpClient, serverClient, message);
                    },

                    ClosedCallback = (client, closedByRemote) => ServerEvents.Get.OnConnectionClosed.Invoke(client, closedByRemote),
                }.RunAsync()
        };
        server.Message += (s, a) => Logger.Debug("Server: " + a.Message);
        var serverTask = server.RunAsync();
        await serverTask;
        IsRunning = false;
    }

    private void RegisterEvents()
    {
        ServerEvents.Get.OnApplicationBoot += args =>
        {
            Logger.Info($"Starting server at {DateTime.Now} on {Environment.MachineName}.");
            return Task.CompletedTask;
        };

        ServerEvents.Get.OnApplicationBoot += async args =>
        {
            await Task.Run(() =>
            {
                // Console.Beep() only works on Windows.
                if (!OperatingSystem.IsWindows()) return;
                Console.Beep(262, 200); // Approx C4.
                Console.Beep(330, 200); // Approx E4.
                Console.Beep(392, 200); // Approx G4.
            });
        };

        ServerEvents.Get.OnConnect += async (sender, client, reconnected) =>
        {
            await Task.Delay(500);
            byte[] bytes =
                Encoding.UTF8.GetBytes($"Hello, {sender.Client.RemoteEndPoint}, my name is Server. Talk to me.");
            await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        };

        ServerEvents.Get.OnReceiveJsonString += ProcessReceivedString;

        ServerEvents.Get.OnConnectionClosed += (client, closedByRemote) =>
        {
            Logger.Info($"Connection closed by remote: {closedByRemote}");
            return Task.CompletedTask;
        };

        ServerEvents.Get.OnApplicationExit += args =>
        {
            Logger.Info("Shutting down server.");
            NLog.LogManager.Shutdown();
            return Task.CompletedTask;
        };

        ServerEvents.Get.OnReceiveMessage.AddHandler<Message.DebugPrint>(async (sender, client, message) =>
        {
            var bytes = Encoding.UTF8.GetBytes($"You said in {typeof(Message.DebugPrint)}: " + message.Text);
            await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        });

        ServerEvents.Get.OnReceiveMessage.AddHandler<Message.MessageTest>(async (sender, client, message) =>
        {
            var bytes = Encoding.UTF8.GetBytes($"You said in {typeof(Message.MessageTest)}: {message.IntegerTest}, {message.DoubleTest}, {message.FloatTest}");
            await client.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        });
    }

    private async Task ProcessReceivedString(TcpClient sender, AsyncTcpClient client, [StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        var messageParam = PrescripshunLib.Networking.Message.GetMessageFromJsonString(jsonString);
        await ServerEvents.Get.OnReceiveMessage.Invoke(sender, client, messageParam);
    }
}