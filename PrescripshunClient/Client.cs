using PrescripshunLib.Logging;
using PrescripshunLib.Networking;
using System.Text;
using Unclassified.Net;

namespace PrescripshunClient;

internal class Client : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    private static void Main(string[] args)
    {
        LogHandler.Configure("client");

        // First we create an instance of the client and register all the events.
        var client = new Client();
        client.RegisterEvents();

        // Then we run the logic.
        ClientEvents.Get.OnApplicationBoot.Invoke(args);
        client.RunClient().GetAwaiter().GetResult();

        ClientEvents.Get.OnApplicationExit.Invoke(args);
    }

    private async Task RunClient()
    {
        var client = new AsyncTcpClient
        {
            IPAddress = NetworkHandler.LocalIpAddress,
            Port = NetworkHandler.Port,
            //AutoReconnect = true,

            // ON CONNECT:
            ConnectedCallback = async (c, isReconnected) =>
            {
                await c.WaitAsync(); // Wait for server banner
                await Task.Delay(50); // Let the banner land in the console window
                Console.WriteLine("Client: type a message at the prompt, or empty to quit.");
                while (true)
                {
                    Console.Write("> ");
                    var consoleReadCts = new CancellationTokenSource();
                    var consoleReadTask = Console.In.ReadLineAsync(consoleReadCts.Token).AsTask();

                    // Wait for user input or closed connection
                    var completedTask = await Task.WhenAny(consoleReadTask, c.ClosedTask);
                    if (completedTask == c.ClosedTask)
                    {
                        consoleReadCts.Cancel(); // Closed connection
                        break;
                    }

                    // User input
                    string enteredMessage = await consoleReadTask;
                    if (enteredMessage == "")
                    {
                        c.Disconnect(); // Close the client connection
                        break;
                    }

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
                        toSend = new PrescripshunLib.Networking.Message.DebugPrint()
                        {
                            Text = enteredMessage
                        };
                    }


                    if (toSend is null) throw new NullReferenceException();
                    byte[] bytes = Encoding.UTF8.GetBytes(toSend.ToJsonString());
                    await c.Send(new ArraySegment<byte>(bytes, 0, bytes.Length));

                    // Wait for server response or closed connection
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
                string message = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                ClientEvents.Get.OnReceive.Invoke(c, message);
                return Task.CompletedTask;
            },

            ClosedCallback = (client, closedByRemote) => ClientEvents.Get.OnConnectionClosed.Invoke(client, closedByRemote),
        };
        client.Message += (s, a) => Logger.Debug("Client: " + a.Message);
        var clientTask = client.RunAsync();

        await clientTask;
    }

    private void RegisterEvents()
    {
        ClientEvents.Get.OnApplicationBoot += async args =>
        {
            Logger.Info($"Starting client at {DateTime.Now} on {Environment.MachineName}.");
        };
        ClientEvents.Get.OnReceive += async (client, text) => Logger.Trace("Client: received: " + text);

        ClientEvents.Get.OnConnectionClosed += (client, remote) =>
        {
            Logger.Info($"Connection closed by remote: {remote}");
            return Task.CompletedTask;
        };

        ClientEvents.Get.OnApplicationExit += args =>
        {
            NLog.LogManager.Shutdown();
            return Task.CompletedTask;
        };
    }
}