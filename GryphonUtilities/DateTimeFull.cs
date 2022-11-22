using System.Numerics;
using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public struct DateTimeFull : IFormattable, IEquatable<DateTimeFull>, IComparable<DateTimeFull>,
    IComparisonOperators<DateTimeFull, DateTimeFull, bool>, IAdditionOperators<DateTimeFull, TimeSpan, DateTimeFull>,
    ISubtractionOperators<DateTimeFull, TimeSpan, DateTimeFull>,
    ISubtractionOperators<DateTimeFull, DateTimeFull, TimeSpan>
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

    public override string ToString() => $"{ToDateTimeOffset():o}@{TimeZoneInfo.Id}";

    public static DateTimeFull? Parse(string input)
    {
        string[] parts = input.Split('@');

        if (parts.Length != 2)
        {
            return null;
        }

        string dateTimeOffsetInput = parts[0];
        if (!DateTimeOffset.TryParse(dateTimeOffsetInput, out DateTimeOffset dateTimeOffset))
        {
            return null;
        }

        string timeZoneId = parts[1];
        TimeZoneInfo? timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(info => info.Id == timeZoneId);
        return timeZoneInfo is null ? null : new DateTimeFull(dateTimeOffset, timeZoneInfo);
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
    public DateTime ToUtcDateTime() => ToDateTimeOffset().UtcDateTime;

    public static DateTimeFull Convert(DateTimeFull dateTimeFull, TimeZoneInfo timeZoneInfo)
    {
        return new DateTimeFull(dateTimeFull.ToDateTimeOffset(), timeZoneInfo);
    }

    public static DateTimeFull ConvertToUtc(DateTimeFull dateTimeFull)
    {
        return new DateTimeFull(dateTimeFull.ToDateTimeOffset(), TimeZoneInfo.Utc);
    }

    public static bool operator ==(DateTimeFull left, DateTimeFull right) => left.Equals(right);
    public static bool operator !=(DateTimeFull left, DateTimeFull right) => !left.Equals(right);

    public static bool operator >(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) > 0;
    public static bool operator <(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) < 0;

    public static bool operator >=(DateTimeFull left, DateTimeFull right) => (left > right) || (left == right);
    public static bool operator <=(DateTimeFull left, DateTimeFull right) => (left < right) || (left == right);

    public static DateTimeFull operator+(DateTimeFull left, TimeSpan right)
    {
        return new DateTimeFull(left.ToDateTimeOffset() + right, left.TimeZoneInfo);
    }

    public static DateTimeFull operator-(DateTimeFull left, TimeSpan right)
    {
        return new DateTimeFull(left.ToDateTimeOffset() - right, left.TimeZoneInfo);
    }

    public static TimeSpan operator-(DateTimeFull left, DateTimeFull right)
    {
        return left.ToDateTimeOffset() - right.ToDateTimeOffset();
    }

    public bool Equals(DateTimeFull other)
    {
        return DateOnly.Equals(other.DateOnly) && TimeOnly.Equals(other.TimeOnly)
                                               && TimeZoneInfo.Equals(other.TimeZoneInfo);
    }

    public override bool Equals(object? obj) => obj is DateTimeFull other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(DateOnly, TimeOnly, TimeZoneInfo);

    public int CompareTo(DateTimeFull other) => ToDateTimeOffset().CompareTo(other.ToDateTimeOffset());
}