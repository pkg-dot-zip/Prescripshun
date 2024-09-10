using PrescripshunLib.Networking;
using System.Text;
using PrescripshunLib.Logging;
using Unclassified.Net;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private static void Main(string[] args)
    {
        LogHandler.Configure("server");

        // First we create an instance of the server and register all the events.
        var server = new Server();
        server.RegisterEvents();

        // Then we run the logic.
        ServerEvents.Get.OnApplicationBoot.Invoke(args);
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

                    ConnectedCallback = async (serverClient, isReconnected) =>
                        ServerEvents.Get.OnConnect.Invoke(tcpClient, serverClient, isReconnected),

                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                        Logger.Info("Server: received: " + message +
                                    $" | FROM: {serverClient.ServerTcpClient.Client.RemoteEndPoint}");

                        bytes = Encoding.UTF8.GetBytes("You said: " + message);
                        await serverClient.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));

                        if (message == "bye") serverClient.Disconnect(); // Let the server close the connection

                        ServerEvents.Get.OnReceiveMessage.Invoke(tcpClient, serverClient, message);
                    }
                }.RunAsync()
        };
        server.Message += (s, a) => Logger.Debug("Server: " + a.Message);
        var serverTask = server.RunAsync();

        await serverTask;
    }

    public void RegisterEvents()
    {
        ServerEvents.Get.OnApplicationBoot += async args =>
        {
            Logger.Info(
                $"Starting server at {DateTime.Now} on {Environment.MachineName}.");
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

        ServerEvents.Get.OnConnect += async (client, server, reconnected) =>
        {
            await Task.Delay(500);
            byte[] bytes =
                Encoding.UTF8.GetBytes($"Hello, {client.Client.RemoteEndPoint}, my name is Server. Talk to me.");
            await server.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));
        };

        ServerEvents.Get.OnReceiveMessage += (sender, server, message) =>
        {
            Logger.Trace($"Printing from OnReceive: \"{message}\" - {sender.Client.RemoteEndPoint}");
        };

        ServerEvents.Get.OnApplicationExit += args =>
        {
            NLog.LogManager.Shutdown();
            return Task.CompletedTask;
        };
    }
}