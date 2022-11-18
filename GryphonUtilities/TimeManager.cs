using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public sealed class TimeManager
{
    public TimeManager(string? timeZoneId = null)
    {
        TimeZoneInfo = timeZoneId is null ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTimeFull Now() => ToLocal(DateTimeFull.CreateUtc(DateTimeOffset.UtcNow));

    public DateTimeFull ToLocal(DateTimeFull dateTimeFull)
    {
        if (dateTimeFull.TimeZoneInfo.Equals(TimeZoneInfo))
        {
            return dateTimeFull;
        }

        DateTimeOffset dateTimeOffset = dateTimeFull.ToDateTimeOffset();
        return new DateTimeFull(dateTimeOffset, TimeZoneInfo);
    }

    public static TimeSpan? GetDelayUntil(DateTimeFull? start, TimeSpan delay, DateTimeFull now)
    {
        if (start is null)
        {
            return null;
        }

        DateTimeOffset time = start.Value.ToDateTimeOffset() + delay;
        DateTimeOffset nowOffset = now.ToDateTimeOffset();
        return time <= nowOffset ? null : time - nowOffset;
    }

    public readonly TimeZoneInfo TimeZoneInfo;
}