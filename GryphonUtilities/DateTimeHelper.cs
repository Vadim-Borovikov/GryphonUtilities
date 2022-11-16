using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public static class DateTimeOffsetHelper
{
    public static DateOnly DateOnly(this DateTimeOffset dateTimeOffset)
    {
        return System.DateOnly.FromDateTime(dateTimeOffset.Date);
    }

    public static TimeOnly TimeOnly(this DateTimeOffset dateTimeOffset)
    {
        return System.TimeOnly.FromTimeSpan(dateTimeOffset.TimeOfDay);
    }

    public static DateTimeOffset FromOnly(DateOnly dateOnly, TimeOnly? timeOnly = null,
        TimeZoneInfo? timeZoneInfo = null)
    {
        return new DateTimeOffset(dateOnly.ToDateTime(timeOnly ?? System.TimeOnly.MinValue),
            timeZoneInfo?.BaseUtcOffset ?? TimeSpan.Zero);
    }
}