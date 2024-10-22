namespace Mfm.Application.Helpers;
public static class DateTimeExtensions
{
    private static readonly string[] DateFormats =
    {
        "yyyy-MM-ddTHH:mm:ssZ",
        "yyyy-MM-ddTHH:mm:ss.fffZ",
        "yyyy-MM-ddTHH:mm:ss",
        "yyyy-MM-dd",
        "dd/MM/yyyy HH:mm:ss",
        "dd/MM/yyyy",
        "MM/dd/yyyy HH:mm:ss",
        "MM/dd/yyyy"
    };

    public static DateTime? ToDateTime(this string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTime.TryParseExact(
                dateString,
                DateFormats,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.RoundtripKind,
                out var dateTime))
        {
            return dateTime;
        }

        throw new FormatException($"Invalid date format: {dateString}");
    }
}