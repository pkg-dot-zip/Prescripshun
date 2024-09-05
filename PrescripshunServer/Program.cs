using PrescripshunLib.Networking;

UDPSocket s = new UDPSocket();
s.Server(NetworkHandler.IpAddress, NetworkHandler.Port);

Console.ReadKey();
s._socket.Close();
Console.WriteLine("Closed Server \n Press any key to exit");
Console.ReadKey();