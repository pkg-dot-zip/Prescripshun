// See https://aka.ms/new-console-template for more information
using PrescripshunLib.Networking;

Console.WriteLine("Hello, World!");


UDPSocket c = new UDPSocket();
c.Client(NetworkHandler.IpAddress, NetworkHandler.Port);
c.Send("TEST!");

Console.ReadLine();
c._socket.Close();
Console.WriteLine("Closed client \n Press any key to exit");
Console.ReadKey();