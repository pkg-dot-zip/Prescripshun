using Microsoft.VisualStudio.TestTools.UnitTesting;
using Unclassified.Net;

namespace PrescripshunLib.Tests;

[TestClass()]
public class GenericEventTests
{
    private interface ITest;
    private class Test1 : ITest;

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public async Task Invoke_ChangeToIntToSetTo_ReturnTrue(int intToSetTo)
    {
        var aEvent = new GenericEvent<ITest>();
        int intToChange = int.MinValue;
        aEvent.AddHandler<Test1>((client, message) =>
        {
            intToChange = intToSetTo;
            return Task.CompletedTask;
        });

        await aEvent.Invoke(new AsyncTcpClient(), new Test1());
        Assert.AreEqual(intToSetTo, intToChange);
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(1)]
    [DataRow(2)]
    [DataRow(3)]
    [DataRow(4)]
    public async Task Remove_ChangeToIntToSetTo_ReturnFalse(int intToSetTo)
    {
        var aEvent = new GenericEvent<ITest>();
        int intToChange = int.MinValue;

        aEvent.AddHandler<Test1>(MyBeautifulFunction);
        aEvent.RemoveHandler<Test1>(MyBeautifulFunction);

        await aEvent.Invoke(new AsyncTcpClient(), new Test1());
        Assert.AreNotEqual(intToSetTo, intToChange);
        return;

        Task MyBeautifulFunction(AsyncTcpClient client, ITest message)
        {
            intToChange = intToSetTo;
            return Task.CompletedTask;
        }
    }
}