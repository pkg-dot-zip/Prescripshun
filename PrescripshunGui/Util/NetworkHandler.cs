using System.Threading.Tasks;
using PrescripshunClient;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util;

public static class NetworkHandler
{
    public static Client Client { get; } = new Client();

    public static void Init() => Client.RunClientForGui();

    public static async Task Send(IMessage message) => await Client.TcpClient.Send(message);
}