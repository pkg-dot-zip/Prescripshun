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
    public void EncryptDecrypt_DebugPrint_Equals_ReturnTrue(bool prefix, string text)
    {
        var message = new Message.DebugPrint {PrintPrefix = prefix, Text = text};
        var encryptedMessage = message.Encrypt();
        var decryptedMessage = Message.GetMessageFromJsonString(encryptedMessage.Decrypt());

        Assert.AreEqual(message, decryptedMessage);
    }
}