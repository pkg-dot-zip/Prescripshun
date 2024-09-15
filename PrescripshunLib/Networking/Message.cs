using Newtonsoft.Json;
using System.Reflection;

namespace PrescripshunLib.Networking
{
    public static class Message
    {
        private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public static IMessage GetMessageFromJsonString(string jsonString)
        {
            var messages = GetAll();

            foreach (var message in messages)
            {
                if (message.InitializeFromJsonString(jsonString, out var message1))
                {
                    Logger.Info($"Deserialized message type found in {nameof(GetMessageFromJsonString)}: {message1.GetType()}");
                    return message1;
                }
            }

            throw new ArgumentException("Invalid jsonString cannot be parsed into a message!");
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
