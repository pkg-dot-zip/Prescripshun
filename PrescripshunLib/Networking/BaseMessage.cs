using Newtonsoft.Json;

namespace PrescripshunLib.Networking
{
    public abstract class BaseMessage : IMessage
    {
        public abstract bool InitializeFromJsonString(string jsonString, out IMessage message);

        public string ToJsonString() => JsonConvert.SerializeObject(this);
    }
}
