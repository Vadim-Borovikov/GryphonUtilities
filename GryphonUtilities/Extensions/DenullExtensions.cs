using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class DenullExtensions
{
    public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T?> items)
    {
        foreach (T? i in items)
        {
            if (i is not null)
            {
                yield return i;
            }
        }
    }
    public static IEnumerable<T> SkipNulls<T>(this IEnumerable<T?> items) where T : struct
    {
        foreach (T? i in items)
        {
            if (i.HasValue)
            {
                yield return i.Value;
            }
        }
    }

    public static T Denull<T>(this T? item, string message)
    {
        return item ?? throw new NullReferenceException(message);
    }
    public static T Denull<T>(this T? item, string message) where T : struct
    {
        return item ?? throw new NullReferenceException(message);
    }

    public static List<T>? TryDenullAll<T>(this IEnumerable<T?> items)
    {
        List<T> result = new();
        foreach (T? item in items)
        {
            if (item is null)
            {
                return null;
            }
            result.Add(item);
        }
        return result;
    }

    public static List<T>? TryDenullAll<T>(this IEnumerable<T?> items) where T : struct
    {
        List<T> result = new();
        foreach (T? item in items)
        {
            if (item is null)
            {
                return null;
            }
            result.Add(item.Value);
        }
        return result;
    }
}