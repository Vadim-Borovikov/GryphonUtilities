using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public sealed class TimeManager
{
    public TimeManager(string? timeZoneId = null)
    {
        TimeZoneInfo = timeZoneId is null ? TimeZoneInfo.Local : TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
    }

    public DateTimeFull Now() => DateTimeFull.CreateNow(TimeZoneInfo);

    public DateTimeFull Convert(DateTimeFull dateTimeFull) => DateTimeFull.Convert(dateTimeFull, TimeZoneInfo);

    public DateTimeFull GetDateTimeFull(DateTime dateTime)
    {
        TimeZoneInfo timeZoneInfo = dateTime.Kind == DateTimeKind.Utc ? TimeZoneInfo.Utc : TimeZoneInfo;
        return new DateTimeFull(DateOnly.FromDateTime(dateTime), TimeOnly.FromDateTime(dateTime), timeZoneInfo);
    }

    public DateTimeFull? ParseDateTimeFull(string input)
    {
        DateTimeFull? parsed = DateTimeFull.Parse(input);
        if (parsed is not null)
        {
            return parsed;
        }
        return DateTimeOffset.TryParse(input, out DateTimeOffset dateTimeOffset)
            ? GetDateTimeFull(dateTimeOffset)
            : null;
    }

    public DateTimeFull GetDateTimeFull(DateTimeOffset dateTimeOffset) => new(dateTimeOffset, TimeZoneInfo);
    public DateTimeFull GetDateTimeFull(DateOnly dateOnly, TimeOnly timeOnly) => new(dateOnly, timeOnly, TimeZoneInfo);

    public static TimeSpan? GetDelayUntil(DateTimeFull? start, TimeSpan delay, DateTimeFull now)
    {
        if (start is null)
        {
            return null;
        }

        DateTimeFull time = start.Value + delay;
        return time <= now ? null : time - now;
    }

    public readonly TimeZoneInfo TimeZoneInfo;
}