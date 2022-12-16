using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class WrapExtensions
{
    public static List<T> WrapWithList<T>(this T item) => new() { item };

    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}