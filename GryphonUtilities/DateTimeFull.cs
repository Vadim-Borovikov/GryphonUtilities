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

        dateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo);
        DateOnly = DateOnly.FromDateTime(dateTimeOffset.DateTime);
        TimeOnly = TimeOnly.FromDateTime(dateTimeOffset.DateTime);
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

    public override string ToString() => $"{ToDateTimeOffset()} {TimeZoneInfo.Id}";

    public static DateTimeFull? Parse(string input)
    {
        string[] parts = input.Split(' ');

        string dateTimeOffsetInput = string.Join(' ', parts.Take(3));
        if (!DateTimeOffset.TryParse(dateTimeOffsetInput, out DateTimeOffset dateTimeOffset))
        {
            return null;
        }

        string timeZoneId = string.Join(' ', parts.Skip(3));
        if (TimeZoneInfo.GetSystemTimeZones().All(info => info.Id != timeZoneId))
        {
            return null;
        }

        return new DateTimeFull(dateTimeOffset, timeZoneId);
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        return ToDateTimeOffset().ToString(format, formatProvider);
    }

    public static DateTimeFull CreateUtc(DateOnly dateOnly, TimeOnly timeOnly)
    {
        return new DateTimeFull(dateOnly, timeOnly, TimeZoneInfo.Utc);
    }

    public static DateTimeFull CreateUtc(DateTimeOffset dateTimeOffset) => new(dateTimeOffset, TimeZoneInfo.Utc);

    public static DateTimeFull CreateNow(TimeZoneInfo timeZoneInfo) => new(DateTimeOffset.Now, timeZoneInfo);

    public static DateTimeFull CreateUtcNow() => CreateUtc(DateTimeOffset.UtcNow);

    public DateTimeOffset ToDateTimeOffset() => new(DateOnly.ToDateTime(TimeOnly), TimeZoneInfo.BaseUtcOffset);

    public static DateTimeFull Convert(DateTimeFull dateTimeFull, TimeZoneInfo timeZoneInfo)
    {
        return new DateTimeFull(dateTimeFull.ToDateTimeOffset(), timeZoneInfo);
    }

    public static DateTimeFull ConvertToUtc(DateTimeFull dateTimeFull)
    {
        return new DateTimeFull(dateTimeFull.ToDateTimeOffset(), TimeZoneInfo.Utc);
    }
}