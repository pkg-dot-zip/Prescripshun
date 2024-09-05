using System.Net;

namespace PrescripshunLib.Networking
{
    public interface ISocket
    {
        void Send(string text);
        public void SendTo(string text, EndPoint endPoint);
        void Receive(IReceiveCallback? callback = null);
    }
}
