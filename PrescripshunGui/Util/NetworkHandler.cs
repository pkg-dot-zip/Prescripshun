using System.Threading.Tasks;
using PrescripshunClient;
using PrescripshunLib.ExtensionMethods;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunGui.Util;

/// <summary>
/// Utility class for handling the Client networking project from the GUI project.
/// </summary>
public static class NetworkHandler
{
    public static Client Client { get; } = new Client();

    /// <summary>
    /// Stars the networking thread from the client networking project.
    /// </summary>
    public static void Init() => Client.RunClientForGui();

    /// <summary>
    /// Helper method that calls <seealso cref="AsyncTcpClientExtensions.Send"/> under the hood with <paramref name="message"/>.
    /// </summary>
    /// <param name="message">Message to send.</param>
    /// <returns></returns>
    public static async Task Send(IMessage message) => await Client.TcpClient.Send(message);
}