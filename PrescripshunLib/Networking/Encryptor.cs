using System.Diagnostics.CodeAnalysis;
using System.Text;
using PrescripshunLib.ExtensionMethods;

namespace PrescripshunLib.Networking
{

    public static class Encryptor
    {
        private static readonly Encoding Encoding = Encoding.UTF8;

        public static byte[] Encrypt(this IMessage message) => Encrypt(message.ToJsonString());

        public static byte[] Encrypt([StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
        {
            // First we encode.
            byte[] encodedString = Encoding.GetBytes(jsonString);

            // Then we encrypt.
            // TODO: Implement encryption.

            return encodedString;
        }

        public static string Decrypt(this byte[] bytes) => Decrypt(bytes, bytes.Length);

        public static string Decrypt(this byte[] bytes, int count, int index = 0)
        {
            // First we decode.
            string decodedString = Encoding.GetString(bytes, index, count);

            // Then we decrypt.
            // TODO: Implement decryption.

            return decodedString;
        }
    }
}
