namespace AuToolbox.Core.Extensions;

public static class TimeSpanExtensions
{
    public static string ToReadableString(this TimeSpan timeSpan) => 
        timeSpan.ToString(@"hh\:mm\:ss");
}