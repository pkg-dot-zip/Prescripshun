using PrescripshunLib.Networking;

namespace PrescripshunServer;

internal class Server : IReceiveCallback
{
    private static void Main(string[] args) => new Server().Boot();

    private NetworkServer _server = null!;

    private void Boot()
    {
        _server = new NetworkServer(NetworkHandler.IpAddress, NetworkHandler.Port, this);

        Console.ReadKey();
        _server._socket.Close();
        Console.WriteLine("Closed Server \n Press any key to exit");
        Console.ReadKey();
    }

    public void OnReceive(ReceivedArgs args)
    {
        Console.WriteLine("SERVER RECEIVED: {0}: {1}, {2}", args.EndPointReceivedFrom, args.Bytes,
            args.Text);

        if (args.Text.Equals("TEST!"))
        {
            _server.SendTo("TEST WORKED!", args.EndPointReceivedFrom);
        }
    }
}