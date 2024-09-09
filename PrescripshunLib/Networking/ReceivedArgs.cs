using System.Net;

namespace PrescripshunLib.Networking
{
    public readonly struct ReceivedArgs(EndPoint endpointReceivedFrom, string text)
    {
        public EndPoint EndPointReceivedFrom { get; } = endpointReceivedFrom;
        public string Text { get; } = text;
    }
}
