using PrescripshunLib.Networking;
using System.Diagnostics.CodeAnalysis;
using Unclassified.Net;
using PrescripshunLib.Networking.Messages;
using PrescripshunLib.Models.User;

namespace PrescripshunClient;

public class Client : AsyncTcpClient
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    public Guid UserKey { get; set; } = Guid.Empty;
    public AsyncTcpClient TcpClient { get; private set; }

    public async Task RunClientForGui()
    {
        TcpClient = new AsyncTcpClient
        {
            IPAddress = NetworkConfig.LocalIpAddress,
            Port = NetworkConfig.Port,
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

    public void RegisterEvents()
    {
        Logger.Info("Registering events in {0}", nameof(Client));

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
    }

    private async Task ProcessReceivedString(AsyncTcpClient client, [StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        var messageParam = PrescripshunLib.Networking.Messages.Message.GetMessageFromJsonString(jsonString);
        await ClientEvents.Get.OnReceiveMessage.Invoke(client, messageParam);
    }
}