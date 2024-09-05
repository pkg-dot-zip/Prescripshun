using System.Net;

namespace PrescripshunLib.Networking
{
    public readonly struct ReceivedArgs(EndPoint endpointReceivedFrom, int bytes, string text)
    {
        public EndPoint EndPointReceivedFrom { get; } = endpointReceivedFrom;
        public int Bytes { get; } = bytes;
        public string Text { get; } = text;
    }
}
