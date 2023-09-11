using System.Numerics;
using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public struct DateTimeFull : IFormattable, IEquatable<DateTimeFull>, IComparable<DateTimeFull>,
    IComparisonOperators<DateTimeFull, DateTimeFull, bool>, IAdditionOperators<DateTimeFull, TimeSpan, DateTimeFull>,
    ISubtractionOperators<DateTimeFull, TimeSpan, DateTimeFull>,
    ISubtractionOperators<DateTimeFull, DateTimeFull, TimeSpan>
{
    public readonly DateOnly DateOnly;
    public readonly TimeOnly TimeOnly;
    public readonly TimeZoneInfo TimeZoneInfo;

    public readonly DateTimeOffset DateTimeOffset;

    public DateTime UtcDateTime => DateTimeOffset.UtcDateTime;


    public DateTimeFull(DateTimeOffset dateTimeOffset, string timeZoneId)
        : this(dateTimeOffset, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
    { }

    public DateTimeFull(DateTimeOffset dateTimeOffset, TimeZoneInfo timeZoneInfo)
    {
        TimeZoneInfo = timeZoneInfo;

        DateTimeOffset = TimeZoneInfo.ConvertTime(dateTimeOffset, TimeZoneInfo);
        DateOnly = DateOnly.FromDateTime(DateTimeOffset.DateTime);
        TimeOnly = TimeOnly.FromDateTime(DateTimeOffset.DateTime);
    }

    public DateTimeFull(DateOnly dateOnly, TimeOnly timeOnly, string timeZoneId)
        : this(dateOnly, timeOnly, TimeZoneInfo.FindSystemTimeZoneById(timeZoneId))
    { }

    public DateTimeFull(DateOnly dateOnly, TimeOnly timeOnly, TimeZoneInfo timeZoneInfo)
    {
        DateOnly = dateOnly;
        TimeOnly = timeOnly;
        TimeZoneInfo = timeZoneInfo;
        DateTimeOffset = new DateTimeOffset(DateOnly.ToDateTime(TimeOnly), TimeZoneInfo.BaseUtcOffset);
    }

    public override string ToString() => $"{DateTimeOffset:o}@{TimeZoneInfo.Id}";

    public static DateTimeFull Parse(string value)
    {
        if (!TryParse(value, out DateTimeFull result))
        {
            throw new FormatException($"String \"{value}\" was not recognized as a valid {nameof(DateTimeFull)}.");
        }
        return result;
    }

    public static bool TryParse(string? value, out DateTimeFull result)
    {
        result = default;

        if (value is null)
        {
            return false;
        }

        string[] parts = value.Split('@');

        if (parts.Length != 2)
        {
            return false;
        }

        string dateTimeOffsetInput = parts[0];
        if (!DateTimeOffset.TryParse(dateTimeOffsetInput, out DateTimeOffset dateTimeOffset))
        {
            return false;
        }

        string timeZoneId = parts[1];
        TimeZoneInfo? timeZoneInfo = TimeZoneInfo.GetSystemTimeZones().SingleOrDefault(info => info.Id == timeZoneId);
        if (timeZoneInfo is null)
        {
            return false;
        }

        result = new DateTimeFull(dateTimeOffset, timeZoneInfo);
        return true;
    }

    public string ToString(string? format, IFormatProvider? formatProvider = null)
    {
        return DateTimeOffset.ToString(format, formatProvider);
    }

    public static DateTimeFull CreateUtc(DateOnly dateOnly, TimeOnly timeOnly)
    {
        return new DateTimeFull(dateOnly, timeOnly, TimeZoneInfo.Utc);
    }

    public static DateTimeFull CreateUtc(DateTimeOffset dateTimeOffset) => new(dateTimeOffset, TimeZoneInfo.Utc);

    public static DateTimeFull CreateNow(TimeZoneInfo timeZoneInfo) => new(DateTimeOffset.Now, timeZoneInfo);

    public static DateTimeFull CreateUtcNow() => new(DateTimeOffset.UtcNow, TimeZoneInfo.Utc);

    public static DateTimeFull Convert(DateTimeFull dateTimeFull, TimeZoneInfo timeZoneInfo)
    {
        return new DateTimeFull(dateTimeFull.DateTimeOffset, timeZoneInfo);
    }

    public static DateTimeFull ConvertToUtc(DateTimeFull dateTimeFull) => Convert(dateTimeFull, TimeZoneInfo.Utc);

    public static bool operator ==(DateTimeFull left, DateTimeFull right) => left.Equals(right);
    public static bool operator !=(DateTimeFull left, DateTimeFull right) => !left.Equals(right);

    public static bool operator >(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) > 0;
    public static bool operator <(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) < 0;

    public static bool operator >=(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) >= 0;
    public static bool operator <=(DateTimeFull left, DateTimeFull right) => left.CompareTo(right) <= 0;

    public static DateTimeFull operator+(DateTimeFull left, TimeSpan right)
    {
        return new DateTimeFull(left.DateTimeOffset + right, left.TimeZoneInfo);
    }

    public static DateTimeFull operator-(DateTimeFull left, TimeSpan right)
    {
        return new DateTimeFull(left.DateTimeOffset - right, left.TimeZoneInfo);
    }

    public static TimeSpan operator-(DateTimeFull left, DateTimeFull right) => left.UtcDateTime - right.UtcDateTime;

    public bool Equals(DateTimeFull other)
    {
        return UtcDateTime.Equals(other.UtcDateTime) && TimeZoneInfo.Equals(other.TimeZoneInfo);
    }

    public override bool Equals(object? obj) => obj is DateTimeFull other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(UtcDateTime, TimeZoneInfo);

    public int CompareTo(DateTimeFull other)
    {
        int utcDiff = UtcDateTime.CompareTo(other.UtcDateTime);
        return utcDiff != 0
            ? utcDiff
            : string.Compare(TimeZoneInfo.Id, other.TimeZoneInfo.Id, StringComparison.InvariantCulture);
    }
}