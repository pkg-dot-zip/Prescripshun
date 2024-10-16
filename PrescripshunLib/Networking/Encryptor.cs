using System.Diagnostics.CodeAnalysis;
using System.Text;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunLib.Networking;

/// <summary>
/// Handles (or delegates) all tasks related to encoding, encrypting & decoding and decrypting <seealso cref="IMessage"/> instances.
/// In other words, turning usable data into bytes and vice versa is handled here.
/// </summary>
public static class Encryptor
{
    private static readonly Encoding Encoding = Encoding.UTF8; // UTF8 seems fine. If you need something else feel free to change it.

    /// <summary>
    /// Simple extension method that calls <seealso cref="Encrypt(string)"/>
    /// automatically from an instance of a subclass of <seealso cref="IMessage"/>.
    /// </summary>
    /// <param name="message">Message to turn into a <seealso cref="string"/> using <seealso cref="Message.ToJsonString"/></param>
    /// <returns></returns>
    public static byte[] Encrypt(this IMessage message) => Encrypt(message.ToJsonString());

    /// <summary>
    /// Turns a <seealso cref="string"/> into <seealso cref="byte"/>s. Follows the following steps:<br/>
    /// 1. Firstly the <paramref name="jsonString"/> is encoded using the default <see cref="Encoding"/>. <br/>
    /// 2. Then the <paramref name="jsonString"/> is encrypted. <br/>
    /// </summary>
    /// <param name="jsonString"><seealso cref="string"/> to convert into <seealso cref="byte"/>s. Will always be in json format for our application, but works for any <seealso cref="string"/>.</param>
    /// <returns></returns>
    public static byte[] Encrypt([StringSyntax(StringSyntaxAttribute.Json)] string jsonString)
    {
        // First we encode.
        byte[] encodedString = Encoding.GetBytes(jsonString);

        // Then we encrypt.
        // TODO: Implement encryption.

        return encodedString;
    }

    /// <summary>
    /// Turns <seealso cref="byte"/>s into a <seealso cref="string"/>. Follows the following steps:<br/>
    /// 1. Firstly the <paramref name="bytes"/> are decrypted. <br/>
    /// 2. Then the <paramref name="bytes"/> are decoded using the default <see cref="Encoding"/>. <br/>
    /// </summary>
    /// <param name="bytes"><seealso cref="byte"/>s to convert into a <seealso cref="string"/>. Will always in result in json format for our application.</param>
    /// <returns></returns>
    public static string Decrypt(this byte[] bytes) => Decrypt(bytes, bytes.Length);

    private static string Decrypt(this byte[] bytes, int count, int index = 0)
    {
        // First we decode.
        string decodedString = Encoding.GetString(bytes, index, count);

        // Then we decrypt.
        // TODO: Implement decryption.

        return decodedString;
    }
}