using PrescripshunLib.Networking;
using PrescripshunLib.Logging;
using Unclassified.Net;
using System.Diagnostics.CodeAnalysis;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Util.Sound;
using PrescripshunServer.Database;

namespace PrescripshunServer;

internal class Server : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
    private static readonly IDatabaseHandler DatabaseHandler = new SqlDatabaseHandler();


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
                        ServerEvents.Get.OnConnect.Invoke(serverClient, isReconnected),

                    ReceivedCallback = async (serverClient, count) =>
                    {
                        byte[] bytes = serverClient.ByteBuffer.Dequeue(count);
                        string jsonString = bytes.Decrypt();
                        Logger.Info("Server: received: " + jsonString +
                                    $" | FROM: {serverClient.ServerTcpClient.Client.RemoteEndPoint}");

                        if (jsonString == "bye") serverClient.Disconnect(); // Let the server close the connection.

                        ServerEvents.Get.OnReceiveJsonString.Invoke(serverClient, jsonString);
                    },

                    ClosedCallback = (client, closedByRemote) =>
                        ServerEvents.Get.OnConnectionClosed.Invoke(client, closedByRemote),
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

        ServerEvents.Get.OnApplicationBoot += async args => await Beeper.PlayServerBootSoundAsync();

        ServerEvents.Get.OnApplicationBoot += async args => await DatabaseHandler.Run();

        ServerEvents.Get.OnConnect += async (client, reconnected) =>
        {
            await Task.Delay(500);
            await client.Send(new Message.DebugPrint()
            {
                Text = $"Hello, {client.ServerTcpClient.Client.RemoteEndPoint}, my name is Server. Talk to me.",
                PrintPrefix = false
            });
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

        ServerEvents.Get.OnReceiveMessage.AddHandler<Message.DebugPrint>((client, message) =>
        {
            Logger.Info("{0}", message.GetPrintString());
            return Task.CompletedTask;
        });

        ServerEvents.Get.OnReceiveMessage.AddHandler<Message.DebugPrint>(async (client, message) =>
        {
            await client.Send(new Message.DebugPrint()
            {
                Text = $"You said in {typeof(Message.DebugPrint)}: " + message.Text
            });
        });

        ServerEvents.Get.OnReceiveMessage.AddHandler<Message.MessageTest>(async (client, message) =>
        {
            await client.Send(new Message.DebugPrint()
            {
                Text =
                    $"You said in {typeof(Message.MessageTest)}: {message.IntegerTest}, {message.DoubleTest}, {message.FloatTest}"
            });
        });
    }

    private async Task ProcessReceivedString(AsyncTcpClient client,
        [StringSyntax(StringSyntaxAttribute.Json)]
        string jsonString)
    {
        var messageParam = PrescripshunLib.Networking.Message.GetMessageFromJsonString(jsonString);
        await ServerEvents.Get.OnReceiveMessage.Invoke(client, messageParam);
    }
}