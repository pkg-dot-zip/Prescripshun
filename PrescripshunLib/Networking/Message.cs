using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace PrescripshunLib.Networking
{
    public static class Message
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static IMessage GetMessageFromJsonString(string jsonString)
        {
            try
            {
                // Parse the JSON string into a JObject to inspect its structure
                var jsonObject = JObject.Parse(jsonString);

                // Check if there is a "Type" field to distinguish the message type
                var typeToken = jsonObject["type"];
                if (typeToken == null)
                {
                    throw new ArgumentException("Invalid JSON: No 'type' field found.");
                }

                // Get the type name from the "type" field
                string messageType = typeToken.ToString();

                Logger.Info($"Received message type: {messageType}");

                // Find the matching message type from our known types
                var messageClass = GetMessageTypeByName(messageType);
                if (messageClass == null)
                {
                    throw new ArgumentException($"Unknown message type: {messageType}");
                }

                // Deserialize the JSON string into the correct message type
                var deserializedMessage = (IMessage)JsonConvert.DeserializeObject(jsonString, messageClass);
                if (deserializedMessage == null)
                {
                    throw new JsonException("Failed to deserialize JSON into the correct message type.");
                }

                Logger.Info($"Successfully deserialized message of type {deserializedMessage.GetType().Name}");
                return deserializedMessage;
            }
            catch (JsonException ex)
            {
                Logger.Error(ex, "Failed to parse JSON string.");
                throw;
            }
        }

        // This method finds the message class by its type name
        private static Type GetMessageTypeByName(string messageType)
        {
            // Get all the message types that inherit from BaseMessage
            var assembly = Assembly.GetExecutingAssembly();
            var messageTypes = assembly.GetTypes()
                .Where(t => typeof(IMessage).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

            // Find the type that matches the name
            return messageTypes.FirstOrDefault(t => t.Name.Equals(messageType, StringComparison.OrdinalIgnoreCase));
        }

        private static List<BaseMessage> GetAll()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var types = assembly.GetTypes();

            // Find all types that are subclasses of BaseMessage
            var baseMessageTypes = types
                .Where(t => t is { IsClass: true, IsAbstract: false, IsInterface: false } && t.IsSubclassOf(typeof(BaseMessage)))
                .ToList();

            return baseMessageTypes.Select(type => (BaseMessage)Activator.CreateInstance(type)).OfType<BaseMessage>().ToList();
        }


        public class DebugPrint : BaseMessage
        {
            public string? Text { get; set; }
            public override bool InitializeFromJsonString(string jsonString, out IMessage message)
            {
                DebugPrint input = null;

                try
                {
                    input = JsonConvert.DeserializeObject<DebugPrint>(jsonString) ?? throw new InvalidOperationException();
                }
                catch
                {
                    message = this;
                    return false;
                }


                Text = input.Text;

                message = this;
                return true;
            }
        }

        public class MessageTest : BaseMessage
        {
            public int? IntegerTest { get; set; }
            public double? DoubleTest { get; set; }
            public float? FloatTest { get; set; }

            public override bool InitializeFromJsonString(string jsonString, out IMessage message)
            {
                MessageTest input = null;

                try
                {
                    input = JsonConvert.DeserializeObject<MessageTest>(jsonString) ?? throw new InvalidOperationException();
                }
                catch
                {
                    message = this;
                    return false;
                }


                IntegerTest = input.IntegerTest;
                DoubleTest = input.DoubleTest;
                FloatTest = input.FloatTest;

                message = this;
                return true;
            }
        }
    }
}
