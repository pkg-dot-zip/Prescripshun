using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PrescripshunLib.Networking
{
    public abstract class BaseMessage : IMessage
    {
        public abstract bool InitializeFromJsonString(string jsonString, out IMessage message);

        public string ToJsonString()
        {
            var jsonString = JsonConvert.SerializeObject(this);
            var jsonObject = JObject.Parse(jsonString);             // Parse the serialized string into a JObject so we can modify it.
            jsonObject.Add("type", this.GetType().Name);         // Add the "type" field with the name of the class.
            return jsonObject.ToString(Formatting.None);
        }
    }
}
