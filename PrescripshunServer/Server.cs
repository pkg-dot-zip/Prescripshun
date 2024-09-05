using PrescripshunLib.Networking;

namespace PrescripshunServer;

internal class Server
{
    private static void Main(string[] args) => new Server().Boot();

    private void Boot()
    {
        UDPSocket s = new UDPSocket();
        s.Server(NetworkHandler.IpAddress, NetworkHandler.Port);

        Console.ReadKey();
        s._socket.Close();
        Console.WriteLine("Closed Server \n Press any key to exit");
        Console.ReadKey();
    }
}