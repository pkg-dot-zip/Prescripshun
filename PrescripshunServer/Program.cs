using PrescripshunLib.Networking;

UDPSocket s = new UDPSocket();
s.Server("127.0.0.1", 27000);

Console.ReadKey();
s._socket.Close();
Console.WriteLine("Closed Server \n Press any key to exit");
Console.ReadKey();