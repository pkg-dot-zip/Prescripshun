using PrescripshunLib.Networking;

namespace PrescripshunClient;

internal class Client : IReceiveCallback
{
    private static void Main(string[] args) => new Client().Boot();
    private void Boot()
    {
        Console.WriteLine("Hello, World!");

        UdpSocket c = new NetworkClient(NetworkHandler.IpAddress, NetworkHandler.Port, this);
        c.Send("TEST!");

        Console.ReadLine();
        c.Socket.Close();
        Console.WriteLine("Closed client \n Press any key to exit");
        Console.ReadKey();
    }

    public void OnReceive(ReceivedArgs args)
    {
        Console.WriteLine("CLIENT RECEIVED: {0}: {1}, {2}", args.EndPointReceivedFrom, args.Bytes,
            args.Text);
    }
}