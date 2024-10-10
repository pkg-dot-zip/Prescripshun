using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrescripshunLib.ExtensionMethods.Tests;

[TestClass]
public class DateTimeExtensionMethodsTests
{
    #region GetSqlString

    #region NoException

    [TestMethod]
    public void GetSqlString_2024_2_23_ThrowNoException()
    {
        var value = DateTime.Parse("2024-02-23");
        try
        {
            value.GetSqlString();
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void GetSqlString_2018_8_4_ThrowNoException()
    {
        var value = DateTime.Parse("2018-08-4");
        try
        {
            value.GetSqlString();
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    [TestMethod]
    public void GetSqlString_2014_3_1_ThrowNoException()
    {
        var value = DateTime.Parse("2014-03-1");
        try
        {
            value.GetSqlString();
        }
        catch (Exception)
        {
            Assert.Fail();
        }
    }

    #endregion

    [TestMethod]
    [DataRow("2010-07-14")]
    [DataRow("2021-06-18")]
    [DataRow("2014-03-12")]
    [DataRow("2017-06-26")]
    [DataRow("2022-10-14")]
    public void GetSqlString_Date_ReturnDate_00_00_00(string str)
    {
        var value = DateTime.Parse(str);
        Assert.AreEqual(value.GetSqlString(), $"{str} 00:00:00");
    }

    [TestMethod]
    [DataRow("2010-07-14 12:10:42")]
    [DataRow("2021-06-18 12:10:42")]
    [DataRow("2014-03-12 12:10:42")]
    [DataRow("2017-06-26 12:10:42")]
    [DataRow("2022-10-14 12:10:42")]
    public void GetSqlString_DateTime12_10_42_ReturnDate12_10_42(string str)
    {
        var value = DateTime.Parse(str);
        Assert.AreEqual(value.GetSqlString(), $"{str}");
    }

    #endregion
}