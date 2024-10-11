using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PrescripshunLib.ExtensionMethods.Tests;

[TestClass]
public class IListExtensionsTests
{
    #region AddAll

    [TestMethod]
    [DataRow(new int[] {1, 2, 3})]
    [DataRow(new int[] {8, 2, 3})]
    [DataRow(new int[] {-2398, 21985, 12598})]
    [DataRow(new int[] {-298, 215, -198})]
    public void AddAllEnumerable_IntData_Contains_IntData_ReturnTrue(int[] value)
    {
        var list = new List<int>();
        var listToAddFrom = value.ToList();

        // ReSharper disable once PossibleMultipleEnumeration
        list.AddAll(listToAddFrom);

        if (list.Count == 0) Assert.Fail();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var i in listToAddFrom)
        {
            if (!list.Contains(i)) Assert.Fail();
        }
    }

    [TestMethod]
    [DataRow(new int[] { 1, 2, 3 })]
    [DataRow(new int[] { 8, 2, 3 })]
    [DataRow(new int[] { -2398, 21985, 12598 })]
    [DataRow(new int[] { -298, 215, -198 })]
    public void AddAllParams_IntData_Contains_IntData_ReturnTrue(int[] value)
    {
        var list = new List<int>();

        // ReSharper disable once PossibleMultipleEnumeration
        list.AddAll(value);

        if (list.Count == 0) Assert.Fail();
        // ReSharper disable once ForeachCanBePartlyConvertedToQueryUsingAnotherGetEnumerator
        foreach (var i in value)
        {
            if (!list.Contains(i)) Assert.Fail();
        }
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(123)]
    [DataRow(-1241)]
    [DataRow(int.MaxValue)]
    [DataRow(int.MinValue)]
    public void AddAllEnumerable_Int_EndsWith_Int_ReturnTrue(int value)
    {
        var list = new List<int> { 98, 912, 173, 123 };
        var listToAddFrom = new List<int> {value};

        list.AddAll(listToAddFrom);
        Assert.AreEqual(listToAddFrom.Last(), value);
    }

    [TestMethod]
    [DataRow(1)]
    [DataRow(123)]
    [DataRow(-1241)]
    [DataRow(int.MaxValue)]
    [DataRow(int.MinValue)]
    public void AddAllParams_Int_EndsWith_Int_ReturnTrue(int value)
    {
        var list = new List<int> { 98, 912, 173, 123 };
        var listToAddFrom = new int[] { value };

        list.AddAll(listToAddFrom);
        Assert.AreEqual(listToAddFrom.Last(), value);
    }

    #endregion
}