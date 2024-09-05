using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PrescripshunLib.Networking
{
    public abstract class UdpSocket : ISocket
    {
        public Socket Socket = null!;
        private const int BufSize = 8 * 1024;
        private readonly State _state = new State();
        private EndPoint _epFrom = new IPEndPoint(IPAddress.Any, 0);
        private AsyncCallback _recv = null!;

        public class State
        {
            public byte[] Buffer = new byte[BufSize];
        }

        public void Send(string text)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            Socket.BeginSend(data, 0, data.Length, SocketFlags.None, (ar) =>
            {
                State so = (State)ar.AsyncState;
                int bytes = Socket.EndSend(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, _state);
        }

        public void SendTo(string text, EndPoint endPoint)
        {
            byte[] data = Encoding.ASCII.GetBytes(text);
            Socket.BeginSendTo(data, 0, data.Length, SocketFlags.None, endPoint, (ar) =>
            {
                State so = (State) ar.AsyncState;
                int bytes = Socket.EndSendTo(ar);
                Console.WriteLine("SEND: {0}, {1}", bytes, text);
            }, _state);
        }

        public void Receive(IReceiveCallback callback)
        {
            Socket.BeginReceiveFrom(_state.Buffer, 0, BufSize, SocketFlags.None, ref _epFrom, _recv = (ar) =>
            {
                try
                {
                    State so = (State) ar.AsyncState;
                    int bytes = Socket.EndReceiveFrom(ar, ref _epFrom);
                    Socket.BeginReceiveFrom(so.Buffer, 0, BufSize, SocketFlags.None, ref _epFrom, _recv, so);

                    callback.OnReceive(new ReceivedArgs(_epFrom, bytes, Encoding.ASCII.GetString(so.Buffer, 0, bytes)));
                }
                catch
                {
                }
            }, _state);
        }
    }
}