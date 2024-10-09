﻿using NLog;
using NLog.Layouts;

namespace PrescripshunLib.Logging;

public static class LogHandler
{
    /// <summary>
    /// Configures the logger correctly. Should be called before any other code is run within a project.
    /// </summary>
    /// <param name="fileNamePrefix">Prefix for the log's file name.</param>
    public static void Configure(string fileNamePrefix = "Preschipshun")
    {
        // Layout options: https://nlog-project.org/config/?tab=layout-renderers.

        var fileName = $"log_{fileNamePrefix}_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";

        LogManager.Setup().LoadConfiguration(builder =>
        {
            builder.ForLogger().FilterMinLevel(LogLevel.Trace).WriteToConsole(layout: ConsoleLayout);
            builder.ForLogger().FilterMinLevel(LogLevel.Trace)
                .WriteToFile(fileName: fileName, layout: LogFileLayout);
        });
    }

    private static readonly Layout ConsoleLayout =
        Layout.FromString("${time}|${level:uppercase=true}|${logger}|${threadid}|${message}|${exception:format=tostring}");

    private static readonly Layout LogFileLayout =
        Layout.FromString(
            "${longdate}|${level:uppercase=true}|${logger}|${threadid}|${message}|${exception:format=tostring}");
}