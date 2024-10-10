using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace PrescripshunLib.Networking.Messages;

/// <summary>
/// Responsible for handling the messages for network traffic.
/// </summary>
public static class Message
{
    private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

    /// <summary>
    /// Converts the <paramref name="message"/> into an instance of <see cref="String"/>.
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    public static string ToJsonString(this IMessage message)
    {
        var jsonString = JsonConvert.SerializeObject(message);
        var jsonObject = JObject.Parse(jsonString);        // Parse the serialized string into a JObject so we can modify it.
        jsonObject.Add("type", message.GetType().Name); // Add the "type" field with the name of the class.
        return jsonObject.ToString(Formatting.None);
    }

    /// <summary>
    /// Converts the <paramref name="jsonString"/> into an instance of <see cref="IMessage"/>.
    /// </summary>
    /// <param name="jsonString"></param>
    /// <returns>The appropriate instance of <see cref="IMessage"/> based on the <paramref name="jsonString"/>.</returns>
    /// <exception cref="ArgumentException"></exception>
    public static IMessage GetMessageFromJsonString([StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        try
        {
            // Parse the JSON string into a JObject to inspect its structure.
            var jsonObject = JObject.Parse(jsonString);

            // Check if there is a "Type" field to distinguish the message type.
            var typeToken = jsonObject["type"];
            if (typeToken == null) throw new ArgumentException("Invalid JSON: No 'type' field found.");

            // Get the type name from the "type" field.
            string messageType = typeToken.ToString();

            Logger.Trace($"Received message in {nameof(GetMessageFromJsonString)} of type: {messageType}");

            // Find the matching message type from our known types.
            var messageClass = GetMessageTypeByName(messageType);
            if (messageClass == null) throw new ArgumentException($"Unknown message type: {messageType}");

            // Deserialize the JSON string into the correct message type.
            var deserializedMessage = (IMessage)JsonConvert.DeserializeObject(jsonString, messageClass);
            if (deserializedMessage == null)
                throw new JsonException("Failed to deserialize JSON into the correct message type.");

            Logger.Info(
                $"Deserialized message of type {deserializedMessage.GetType().Name} in {nameof(GetMessageFromJsonString)}");
            return deserializedMessage;
        }
        catch (JsonException ex)
        {
            Logger.Error(ex, "Failed to parse JSON string.");
            throw;
        }
    }

    /// <summary>
    /// Finds the message <see langword="class"/> by its <see langword="type"/>name using <see href="https://learn.microsoft.com/en-us/dotnet/fundamentals/reflection/reflection">reflection</see>.
    /// </summary>
    /// <param name="messageType">Name of <see langword="type"/>.</param>
    /// <returns>Sub<see cref="Type"/> of <see cref="IMessage"/>.</returns>
    private static Type GetMessageTypeByName(string messageType)
    {
        // Get all the message types that inherit from BaseMessage
        var assembly = Assembly.GetExecutingAssembly();
        var messageTypes = assembly.GetTypes()
            .Where(t => typeof(IMessage).IsAssignableFrom(t) && t is { IsAbstract: false, IsInterface: false });

        // Find the type that matches the name
        return messageTypes.FirstOrDefault(t => t.Name.Equals(messageType, StringComparison.OrdinalIgnoreCase));
    }

    public class DebugPrint : IMessage, IEquatable<DebugPrint>
    {
        public string? Text { get; set; }
        public bool PrintPrefix { get; set; } = true;

        public string GetPrintString()
        {
            if (PrintPrefix) return $"DebugPrint: {Text}";
            else return Text ?? string.Empty;
        }

        public bool Equals(DebugPrint? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return Text == other.Text && PrintPrefix == other.PrintPrefix;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DebugPrint) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Text, PrintPrefix);
        }
    }

    public class MessageTest : IMessage, IEquatable<MessageTest>
    {
        public int? IntegerTest { get; set; }
        public double? DoubleTest { get; set; }
        public float? FloatTest { get; set; }

        public bool Equals(MessageTest? other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            return IntegerTest == other.IntegerTest && Nullable.Equals(DoubleTest, other.DoubleTest) && Nullable.Equals(FloatTest, other.FloatTest);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((MessageTest) obj);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(IntegerTest, DoubleTest, FloatTest);
        }
    }
}