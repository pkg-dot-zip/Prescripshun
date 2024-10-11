using System.Globalization;
using Csv;
using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Util;

// TODO: Maybe move to client gui project since this is only used there?
public static class CalenderExport
{
    private class GoogleCalenderAppointment()
    {
        private const int AppointmentEndsAfterStartInMinutes = 15;

        public GoogleCalenderAppointment(Appointment appointment) : this(appointment.Title, appointment.DateTime,
            appointment.Description)
        {
        }

        private GoogleCalenderAppointment(string subject, DateTime startDateTime, string description) : this()
        {
            Subject = subject;
            StartDate = startDateTime.ToString("d");
            StartTime = startDateTime.ToString("t", new CultureInfo("en-US"));
            EndDate = startDateTime.AddMinutes(AppointmentEndsAfterStartInMinutes).ToString("d");
            EndTime = startDateTime.AddMinutes(AppointmentEndsAfterStartInMinutes)
                .ToString("t", new CultureInfo("en-US"));
            Description = description;
        }

        public string? Subject { get; set; } = null;
        public string? StartDate { get; set; } = null;
        public string? StartTime { get; set; } = null;
        public string? EndDate { get; set; } = null;
        public string? EndTime { get; set; } = null;
        public string? AllDayEvent { get; set; } = null;
        public string? Description { get; set; } = null;
        public string? Location { get; set; } = null;
        public string? Private { get; set; } = null;
    }

    private static List<GoogleCalenderAppointment> ConvertToGoogleCalender(this IEnumerable<Appointment> appointments)
    {
        return appointments.Select(appointment => new GoogleCalenderAppointment(appointment)).ToList();
    }

    public static string GetGoogleCalenderAppointmentsExportString(IEnumerable<Appointment> appointments)
    {
        return GetGoogleCalenderAppointmentsExportString(appointments.ConvertToGoogleCalender());
    }

    private static string GetGoogleCalenderAppointmentsExportString(IEnumerable<GoogleCalenderAppointment> appointments)
    {
        var myExport = new CsvExport(
            columnSeparator: ",",
            includeColumnSeparatorDefinitionPreamble: true, //Excel wants this in CSV files!
            includeHeaderRow: true
        );

        foreach (var appointment in appointments)
        {
            myExport.AddRow();
            if (appointment.Subject is not null) myExport["Subject"] = appointment.Subject;
            if (appointment.StartDate is not null) myExport["Start Date"] = appointment.StartDate;
            if (appointment.StartTime is not null) myExport["Start Time"] = appointment.StartTime;
            if (appointment.EndDate is not null) myExport["End Date"] = appointment.EndDate;
            if (appointment.EndTime is not null) myExport["End Time"] = appointment.EndTime;
            if (appointment.AllDayEvent is not null) myExport["All Day Event"] = appointment.AllDayEvent;
            if (appointment.Description is not null) myExport["Description"] = appointment.Description;
            if (appointment.Location is not null) myExport["Location"] = appointment.Location;
            if (appointment.Private is not null) myExport["Private"] = appointment.Private;
        }

        return myExport.Export();
    }
}