using PrescripshunLib.Networking;

namespace PrescripshunClient;

internal class Client
{
    private static void Main(string[] args) => new Client().Boot();
    private void Boot()
    {
        Console.WriteLine("Hello, World!");

        UdpSocket c = new NetworkClient(NetworkHandler.IpAddress, NetworkHandler.Port);
        c.Send("TEST!");

        Console.ReadLine();
        c._socket.Close();
        Console.WriteLine("Closed client \n Press any key to exit");
        Console.ReadKey();
    }
}