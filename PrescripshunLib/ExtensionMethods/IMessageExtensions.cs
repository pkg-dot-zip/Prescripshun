using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunLib.ExtensionMethods;

public static class IMessageExtensions
{
    public static string ToJsonString(this IMessage message)
    {
        var jsonString = JsonConvert.SerializeObject(message);
        var jsonObject = JObject.Parse(jsonString);        // Parse the serialized string into a JObject so we can modify it.
        jsonObject.Add("type", message.GetType().Name); // Add the "type" field with the name of the class.
        return jsonObject.ToString(Formatting.None);
    }
}