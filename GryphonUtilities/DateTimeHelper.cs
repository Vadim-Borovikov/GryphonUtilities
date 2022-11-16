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
}