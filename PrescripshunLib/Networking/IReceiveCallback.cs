namespace PrescripshunLib.Networking
{
    public interface IReceiveCallback
    {
        void OnReceive(ReceivedArgs args);
    }
}
