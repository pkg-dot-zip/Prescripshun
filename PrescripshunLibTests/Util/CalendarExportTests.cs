using Microsoft.VisualStudio.TestTools.UnitTesting;
using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Util.Tests;

[TestClass]
public class CalendarExportTests
{

    // TODO: Create unit tests with special chars: comma (,), period(.), exclamation mark (!) and question mark(?).

    [TestMethod]
    [DataRow("Title1")]
    [DataRow("Title2")]
    [DataRow("Title3")]
    [DataRow("Titel1")]
    [DataRow("Titel2")]
    [DataRow("Titel3")]
    public void GetGoogleCalenderAppointmentsExportString_SimpleChars_CanParseTitleCorrectly_ReturnsTrue(string title, string description = "Beschrijving1", string dateTimeString = "01-01-1999")
    {
        List<Appointment> appointments = [new Appointment()
        {
            Title = title,
            Description = description,
            DateTime = DateTime.Parse(dateTimeString)
        }];

        var exportString = CalendarExport.GetGoogleCalenderAppointmentsExportString(appointments);

        Assert.AreEqual($"Subject,Start Date,Start Time,End Date,End Time,Description\r\n{title},01/01/1999,12:00 AM,01/01/1999,12:15 AM,Beschrijving1\r\n", exportString);
    }

    [TestMethod]
    [DataRow("Desc1")]
    [DataRow("Desc2")]
    [DataRow("Desc3")]
    [DataRow("Beschrijving1")]
    [DataRow("Beschrijving2")]
    [DataRow("Beschrijving3")]
    public void GetGoogleCalenderAppointmentsExportString_SimpleChars_CanParseDescriptionCorrectly_ReturnsTrue(string description, string title = "Titel1", string dateTimeString = "01-01-1999")
    {
        List<Appointment> appointments = [new Appointment()
        {
            Title = title,
            Description = description,
            DateTime = DateTime.Parse(dateTimeString)
        }];

        var exportString = CalendarExport.GetGoogleCalenderAppointmentsExportString(appointments);

        Assert.AreEqual($"Subject,Start Date,Start Time,End Date,End Time,Description\r\nTitel1,01/01/1999,12:00 AM,01/01/1999,12:15 AM,{description}\r\n", exportString);
    }
}