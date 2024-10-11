using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrescripshunLib.ExtensionMethods.Tests;

[TestClass]
public class RandomExtensionsTests
{
    #region NextBool

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(11)]
    [DataRow(22)]
    [DataRow(33)]
    [DataRow(44)]
    [DataRow(123)]
    [DataRow(91238)]
    [DataRow(int.MaxValue)]
    public void NextBool_0_ReturnsTrue(int randomSeed)
    {
        var random = new Random(randomSeed);
        Assert.IsTrue(random.NextBool(0.0D));
    }

    #endregion
}