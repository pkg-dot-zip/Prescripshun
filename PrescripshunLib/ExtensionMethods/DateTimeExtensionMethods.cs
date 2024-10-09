namespace PrescripshunLib.ExtensionMethods;

public static class DateTimeExtensionMethods
{
    /// <summary>
    /// Converts an instance of <see cref="DateTime"/> into a MySql parsable <see cref="String"/>.
    /// </summary>
    /// <param name="dateTime">DateTime to convert into a string</param>
    /// <returns>A <see cref="String"/> representing this <paramref name="dateTime"/> into a MySql parsable format.</returns>
    public static string GetSqlString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
}