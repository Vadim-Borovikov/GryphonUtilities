using JetBrains.Annotations;

namespace GryphonUtilities.Extensions;

[PublicAPI]
public static class StringExtensions
{
    public static string Format(this string s, params object?[] args) => string.Format(s, args);
}