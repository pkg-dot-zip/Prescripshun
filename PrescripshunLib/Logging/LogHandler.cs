using NLog;

namespace PrescripshunLib.Logging
{
    public static class LogHandler
    {
        public static void Configure(string fileNamePrefix = "Preschipshun")
        {
            LogManager.Setup().LoadConfiguration(builder => {
                builder.ForLogger().FilterMinLevel(LogLevel.Trace).WriteToConsole();
                builder.ForLogger().FilterMinLevel(LogLevel.Debug).WriteToFile(fileName: $"log_{fileNamePrefix}_{DateTime.Now}.txt");
            });
        }
    }
}
