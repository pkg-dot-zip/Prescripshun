using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PrescripshunLib.Networking
{
    public abstract class UdpSocket : ISocket
    {
        public Socket _socket;
        private const int bufSize = 8 * 1024;
        private State state = new State();
        private EndPoint epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback recv = null;

        public class State
        {
            public byte[] buffer = new byte[bufSize];
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        public void SendTo(string text, EndPoint endPoint)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            _socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, (ar) =>
            {
                State so = (State) ar.AsyncState;
                int bytes = _socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, state);
        }

        public void Receive(IReceiveCallback callback)
        {
            _socket.BeginReceiveFrom(state.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv = (ar) =>
            {
                try
                {
                    State so = (State) ar.AsyncState;
                    int bytes = _socket.EndReceiveFrom(ar, ref epFrom);
                    _socket.BeginReceiveFrom(so.buffer, 0, bufSize, SocketFlags.None, ref epFrom, recv, so);

                    callback.OnReceive(new ReceivedArgs(epFrom, bytes, Encoding.ASCII.GetString(so.buffer, 0, bytes)));
                }
                catch
                {
                }
            }, state);
        }
    }
}