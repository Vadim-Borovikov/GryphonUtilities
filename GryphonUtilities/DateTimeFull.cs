using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public struct DateTimeFull : IFormattable
{
    public DateOnly DateOnly;
    public TimeOnly TimeOnly;
    public TimeZoneInfo TimeZoneInfo;

    public DateTimeFull(DateTimeOffset dateTimeOffset, string timeZoneId)
        : this(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
    { }

    public DateTimeFull(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        TimeZoneInfo = timeZoneInfo;

        DateTime dateTime = timeZoneInfo.Equals(TimeZoneInfo.Utc)
            ? dateTimeOffset.UtcDateTime
            : TimeZoneInfo.ConvertTimeFromUtc(dateTimeOffset.UtcDateTime, TimeZoneInfo);
        DateOnly = DateOnly.FromDateTime(dateTime);
        TimeOnly = TimeOnly.FromDateTime(dateTime);
    }

    public DateTimeFull(DateOnly dateOnly, TimeOnly timeOnly, string timeZoneId)
        : this(dateOnly, timeOnly, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
    { }

    public DateTimeFull(DateOnly dateOnly, TimeOnly timeOnly, TimeZoneInfo timeZoneInfo)
    {
        DateOnly = dateOnly;
        TimeOnly = timeOnly;
        TimeZoneInfo = timeZoneInfo;
    }

    public override string ToString()
    {
        string o = TimeZoneInfo.BaseUtcOffset.ToString(@"hh\:mm");
        return $"{DateOnly} {TimeOnly} +{o} {TimeZoneInfo.Id}";
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        return ToDateTimeOffset().ToString(format, formatProvider);
    }

    public static DateTimeFull CreateUtc(DateOnly dateOnly, TimeOnly timeOnly)
    {
        return new DateTimeFull(dateOnly, timeOnly, TimeZoneInfo.Utc);
    }

    public static DateTimeFull CreateUtc(DateTimeOffset dateOnlyOffset) => new(dateOnlyOffset, TimeZoneInfo.Utc);

    public DateTimeOffset ToDateTimeOffset() => new(DateOnly.ToDateTime(TimeOnly), TimeZoneInfo.BaseUtcOffset);
}