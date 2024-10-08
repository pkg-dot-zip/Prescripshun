using PrescripshunLib.Logging;
using PrescripshunLib.Networking;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using PrescripshunLib.ExtensionMethods;
using Unclassified.Net;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunClient;

public class Client : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Guid UserKey { get; set; } = Guid.Empty;
    public AsyncTcpClient TcpClient { get; private set; }

    private static void Main(string[] args)
    {
        LogHandler.Configure("client");

        // First we create an instance of the client and register all the ClientEvents.Get.
        var client = new Client();
        client.RegisterEvents();

        // Then we run the logic.
        ClientEvents.Get.OnApplicationBoot.Invoke(args);
        client.RunClient().GetAwaiter().GetResult();

        ClientEvents.Get.OnApplicationExit.Invoke(args);
    }

    private async Task RunClient()
    {
        TcpClient = new AsyncTcpClient
        {
            IPAddress = NetworkHandler.LocalIpAddress,
            Port = NetworkHandler.Port,
            //AutoReconnect = true,

            // ON CONNECT:
            ConnectedCallback = async (c, isReconnected) =>
            {
                await c.WaitAsync(); // Wait for server banner.
                await Task.Delay(50); // Let the banner land in the console window.
                Console.WriteLine("Client: type a message at the prompt, or empty to quit.");
                while (true)
                {
                    Console.Write("> ");
                    var consoleReadCts = new CancellationTokenSource();
                    var consoleReadTask = Console.In.ReadLineAsync(consoleReadCts.Token).AsTask();

                    // Wait for user input or closed connection.
                    var completedTask = await Task.WhenAny(consoleReadTask, c.ClosedTask);
                    if (completedTask == c.ClosedTask)
                    {
                        consoleReadCts.Cancel(); // Closed connection.
                        break;
                    }

                    // User input
                    string enteredMessage = await consoleReadTask;
                    if (enteredMessage == "")
                    {
                        c.Disconnect(); // Close the client connection.
                        break;
                    }

                    if (enteredMessage is not null) await HandleConsoleInput(enteredMessage);

                    // Wait for server response or closed connection.
                    await c.ByteBuffer.WaitAsync();
                    if (c.IsClosing) break;
                }
                // NOTE: The client connection will NOT be closed automatically when this method
                //       returns. It has to be closed explicitly when desired.
            },

            // ON RECEIVE:
            ReceivedCallback = (c, count) => // count = number of bytes received.
            {
                byte[] bytes = c.ByteBuffer.Dequeue(count);
                string jsonString = bytes.Decrypt();
                ClientEvents.Get.OnReceiveJsonString.Invoke(c, jsonString);
                return Task.CompletedTask;
            },

            ClosedCallback = (client, closedByRemote) => ClientEvents.Get.OnConnectionClosed.Invoke(client, closedByRemote),
        };
        TcpClient.Message += (s, a) => Logger.Debug("Client: " + a.Message);
        var clientTask = TcpClient.RunAsync();

        await clientTask;
    }

    public async Task RunClientForGui()
    {
        TcpClient = new AsyncTcpClient
        {
            IPAddress = NetworkHandler.LocalIpAddress,
            Port = NetworkHandler.Port,
            //AutoReconnect = true,

            // ON CONNECT:
            ConnectedCallback = async (c, isReconnected) =>
            {
            },

            // ON RECEIVE:
            ReceivedCallback = (c, count) => // count = number of bytes received.
            {
                byte[] bytes = c.ByteBuffer.Dequeue(count);
                string jsonString = bytes.Decrypt();
                ClientEvents.Get.OnReceiveJsonString.Invoke(c, jsonString);
                return Task.CompletedTask;
            },

            ClosedCallback = (client, closedByRemote) => ClientEvents.Get.OnConnectionClosed.Invoke(client, closedByRemote),
        };
        TcpClient.Message += (s, a) => Logger.Debug("Client: " + a.Message);
        var clientTask = TcpClient.RunAsync();
        await clientTask;
    }

    private async Task HandleConsoleInput(string enteredMessage)
    {
        IMessage toSend = null;


        if (enteredMessage.StartsWith("1"))
        {
            Logger.Info($"Found {typeof(Message.MessageTest)} to send!");
            toSend = new Message.MessageTest()
            {
                IntegerTest = 1,
                DoubleTest = 2.0,
                FloatTest = 3.0F,
            };
        }
        else
        {
            Logger.Info($"Found {typeof(Message.DebugPrint)} to send!");
            toSend = new Message.DebugPrint()
            {
                Text = enteredMessage
            };
        }


        if (toSend is null) throw new NullReferenceException();
        await TcpClient.Send(toSend);
    }

    public void RegisterEvents()
    {
        Logger.Info("Registering events in {0}", nameof(Client));
        ClientEvents.Get.OnApplicationBoot += async args =>
        {
            Logger.Info($"Starting client at {DateTime.Now} on {Environment.MachineName}.");
        };

        ClientEvents.Get.OnConnectionClosed += (client, remote) =>
        {
            Logger.Info($"Connection closed by remote: {remote}");
            return Task.CompletedTask;
        };

        ClientEvents.Get.OnReceiveJsonString += ProcessReceivedString;

        ClientEvents.Get.OnReceiveMessage.AddHandler<Message.DebugPrint>((client, message) =>
        {
            Logger.Info("{0}", message.GetPrintString());
            return Task.CompletedTask;
        });

        ClientEvents.Get.OnApplicationExit += args =>
        {
            NLog.LogManager.Shutdown();
            return Task.CompletedTask;
        };
    }

    private async Task ProcessReceivedString(AsyncTcpClient client, [StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        var messageParam = PrescripshunLib.Networking.Messages.Message.GetMessageFromJsonString(jsonString);
        await ClientEvents.Get.OnReceiveMessage.Invoke(client, messageParam);
    }
}