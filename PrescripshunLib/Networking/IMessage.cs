using System.Text.Json.Nodes;

namespace PrescripshunLib.Networking
{
    public interface IMessage
    {
        public bool InitializeFromJsonString(string jsonString, out IMessage message);
        public string ToJsonString();
    }
}
