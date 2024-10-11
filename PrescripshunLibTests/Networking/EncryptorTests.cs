using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrescripshunLib.Networking.Messages;

namespace PrescripshunLib.Networking.Tests;

[TestClass]
public class EncryptorTests
{
    [TestMethod]
    [DataRow(false, "This is a test")]
    [DataRow(true, "This is a test")]
    [DataRow(false, "Dit is een test")]
    [DataRow(true, "Dit is een test")]
    [DataRow(false, "Dies ist ein Test")]
    [DataRow(true, "Dies ist ein Test")]
    [DataRow(false, "Это тест")]
    [DataRow(true, "Это тест")]
    [DataRow(false, "Esto es una prueba")]
    [DataRow(true, "Esto es una prueba")]
    public void Message_EncryptDecrypt_DebugPrint_Equals_ReturnTrue(bool prefix, string text)
    {
        var message = new Message.DebugPrint { PrintPrefix = prefix, Text = text };
        var encryptedMessage = message.Encrypt();
        var decryptedMessage = Message.GetMessageFromJsonString(encryptedMessage.Decrypt());

        Assert.AreEqual(message, decryptedMessage);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("\n\n\n\n\n")]
    [DataRow("\t\t\t\t\t")]
    [DataRow("abcdefg")]
    [DataRow("ABCDEFG")]
    [DataRow("_-!?.,")]
    [DataRow("éËöåþü")]
    [DataRow("©©©©")]
    [DataRow("Lorem ipsum dolor sit amet")]
    [DataRow("Лорем ипсум долор сит амет")]
    [DataRow("Lorem ipsum dolor sit amet\nLorem ipsum dolor sit amet")]
    [DataRow("Лорем\nипсум\nдолор\nсит\nамет")]
    [DataRow("Lorem\nipsum\ndolor\nsit\namet")]
    public void String_EncryptDecrypt_Equals_ReturnTrue(string content)
    {
        var bytes = Encryptor.Encrypt(content);
        var decryptedString = bytes.Decrypt();
        Assert.AreEqual(content, decryptedString);
    }

    [TestMethod]
    [DataRow("""
             {
             data: "test"
             }
             """)]
    [DataRow("""
             {
             dataA: "test",
             dataB: "test",
             }
             """)]
    [DataRow("""
             {
             dataA: "test1",
             dataB: "test2",
             dataC: "test3"
             }
             """)]
    public void JsonString_EncryptDecrypt_Equals_ReturnTrue([StringSyntax(StringSyntaxAttribute.Json)] string content)
    {
        var bytes = Encryptor.Encrypt(content);
        var decryptedString = bytes.Decrypt();
        Assert.AreEqual(content, decryptedString);
    }
}