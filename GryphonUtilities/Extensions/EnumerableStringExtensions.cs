using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class EnumerableStringExtensions
{
    public static string Join(this IEnumerable<string?> parts, string? separator) => string.Join(separator, parts);
    public static string JoinLines(this IEnumerable<string?> lines) => lines.Join("\n");
}