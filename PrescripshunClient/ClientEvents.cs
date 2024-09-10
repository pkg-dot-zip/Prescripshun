using System.Net.Sockets;
using PrescripshunLib.Networking;
using Unclassified.Net;

namespace PrescripshunClient
{
    internal class ClientEvents
    {
        private static ClientEvents? instance = null;

        private ClientEvents()
        {
        }

        public static ClientEvents Get => instance ??= new ClientEvents();


        public delegate Task OnReceiveDelegate(AsyncTcpClient client, string text);
        public OnReceiveDelegate OnReceive { get; set; }
    }
}