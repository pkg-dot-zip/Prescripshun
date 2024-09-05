using PrescripshunLib.Networking;

namespace PrescripshunServer;

internal class Server
{
    private static void Main(string[] args) => new Server().Boot();

    private void Boot()
    {
        var server = new NetworkServer(NetworkHandler.IpAddress, NetworkHandler.Port);

        Console.ReadKey();
        server._socket.Close();
        Console.WriteLine("Closed Server \n Press any key to exit");
        Console.ReadKey();
    }
}