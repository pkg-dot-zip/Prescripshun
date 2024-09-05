namespace PrescripshunLib.Networking
{
    public interface ISocket
    {
        void Send(string text);
        void Receive();
    }
}
