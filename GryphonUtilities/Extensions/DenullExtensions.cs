using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class DenullExtensions
{
    public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> items)
    {
        return items.Where(i => i is not null).Select(i => i.GetValue());
    }
    public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> items) where T : struct
    {
        return items.Where(i => i is not null).Select(i => i.GetValue());
    }

    public static T GetValue<T>(this T? item, string? message = null)
    {
        return item ?? throw new NullReferenceException(message);
    }
    public static T GetValue<T>(this T? item, string? message = null) where T : struct
    {
        return item ?? throw new NullReferenceException(message);
    }
}