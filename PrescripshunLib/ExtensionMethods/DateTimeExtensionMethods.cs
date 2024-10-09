namespace PrescripshunLib.ExtensionMethods;

public static class DateTimeExtensionMethods
{
    public static string GetSqlString(this DateTime dateTime) => dateTime.ToString("yyyy-MM-dd HH:mm:ss");
}