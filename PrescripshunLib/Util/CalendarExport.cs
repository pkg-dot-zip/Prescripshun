using System.Globalization;
using Csv;
using PrescripshunLib.Models.MedicalFile;

namespace PrescripshunLib.Util;

/// <summary>
/// Handles conversion from <see langword="object"/>s to <see cref="GoogleCalendarAppointment"/> instances.
/// </summary>
public static class CalendarExport
{
    /// <summary>
    /// Contains data for all fields used by Google Calendar.
    /// See <a href="https://support.google.com/calendar/answer/37118?hl=en&co=GENIE.Platform%3DDesktop#zippy=%2Ccreate-or-edit-a-csv-file">this</a>
    /// page for more information on the individual fields.
    ///
    /// Also includes a constructor that converts our <see cref="Appointment"/> class into an instance of this.
    ///
    /// Please note that all fields are represented as strings here.
    /// </summary>
    private class GoogleCalendarAppointment()
    {
        private const int AppointmentEndsAfterStartInMinutes = 15;

        public GoogleCalendarAppointment(Appointment appointment) : this(appointment.Title, appointment.DateTime,
            appointment.Description)
        {
        }

        private GoogleCalendarAppointment(string subject, DateTime startDateTime, string description) : this()
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

    private static List<GoogleCalendarAppointment> ConvertToGoogleCalender(this IEnumerable<Appointment> appointments)
    {
        return appointments.Select(appointment => new GoogleCalendarAppointment(appointment)).ToList();
    }

    /// <summary>
    /// Gets a <i>.csv</i> formatted <see cref="string"/> from an <see cref="IEnumerable{T}"/> of <see cref="Appointment"/>.
    /// This follows the format of Google Calendar.
    /// </summary>
    /// <param name="appointments">Instances of <see cref="Appointment"/> appointments to export as a <see cref="string"/>.</param>
    /// <returns></returns>
    public static string GetGoogleCalenderAppointmentsExportString(IEnumerable<Appointment> appointments)
    {
        return GetGoogleCalenderAppointmentsExportString(appointments.ConvertToGoogleCalender());
    }

    /// <summary>
    /// Gets a <i>.csv</i> formatted <see cref="string"/> from an <see cref="IEnumerable{T}"/> of <see cref="GoogleCalendarAppointment"/>.
    /// This follows the format of Google Calendar.
    /// </summary>
    /// <param name="appointments">Instances of <see cref="GoogleCalendarAppointment"/> appointments to export as a <see cref="string"/>.</param>
    /// <param name="includeColumnSeparatorDefinitionPreamble">Excel wants this in CSV files! However, we are working with Google Calendar, so default is <c>false</c>.</param>
    /// <returns></returns>
    private static string GetGoogleCalenderAppointmentsExportString(IEnumerable<GoogleCalendarAppointment> appointments, bool includeColumnSeparatorDefinitionPreamble = false)
    {
        var exporter = new CsvExport(
            columnSeparator: ",",
            includeColumnSeparatorDefinitionPreamble: includeColumnSeparatorDefinitionPreamble,
            includeHeaderRow: true
        );

        foreach (var appointment in appointments)
        {
            exporter.AddRow();
            if (appointment.Subject is not null) exporter["Subject"] = appointment.Subject;
            if (appointment.StartDate is not null) exporter["Start Date"] = appointment.StartDate;
            if (appointment.StartTime is not null) exporter["Start Time"] = appointment.StartTime;
            if (appointment.EndDate is not null) exporter["End Date"] = appointment.EndDate;
            if (appointment.EndTime is not null) exporter["End Time"] = appointment.EndTime;
            if (appointment.AllDayEvent is not null) exporter["All Day Event"] = appointment.AllDayEvent;
            if (appointment.Description is not null) exporter["Description"] = appointment.Description;
            if (appointment.Location is not null) exporter["Location"] = appointment.Location;
            if (appointment.Private is not null) exporter["Private"] = appointment.Private;
        }

        return exporter.Export();
    }
}