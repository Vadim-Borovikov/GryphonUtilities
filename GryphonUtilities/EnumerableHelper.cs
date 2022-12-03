using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public static class EnumerableHelper
{
    public static IEnumerable<T> Yield<T>(this T item)
    {
        yield return item;
    }
}