using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public static class NullableHelper
{
    public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> seq)
    {
        return seq.Where(i => i is not null).Select(i => i.GetValue());
    }
    public static IEnumerable<T> RemoveNulls<T>(this IEnumerable<T?> seq) where T : struct
    {
        return seq.Where(i => i is not null).Select(i => i.GetValue());
    }

    public static T GetValue<T>(this T? param, string? message = null)
    {
        return param ?? throw new NullReferenceException(message);
    }
    public static T GetValue<T>(this T? param, string? message = null) where T : struct
    {
        return param ?? throw new NullReferenceException(message);
    }
}