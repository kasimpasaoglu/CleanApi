namespace Application.Extensions;

public static class DateTimeOffsetExtensions
{
    private static readonly TimeZoneInfo IstanbulTz =
        TryGetTz("Europe/Istanbul") ?? TryGetTz("Turkey Standard Time") ?? TimeZoneInfo.Utc;

    private static TimeZoneInfo? TryGetTz(string id)
    {
        try { return TimeZoneInfo.FindSystemTimeZoneById(id); }
        catch { return null; }
    }
    
    public static DateOnly ToIstanbulDateOnly(this DateTimeOffset dto)
    {
        var local = TimeZoneInfo.ConvertTime(dto, IstanbulTz).DateTime;
        return DateOnly.FromDateTime(local);
    }
}