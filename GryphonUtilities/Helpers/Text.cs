using GryphonUtilities.Extensions;
using JetBrains.Annotations;

namespace GryphonUtilities.Helpers;

[PublicAPI]
public static class Text
{
    public static string AddWord(string? first, string? second)
    {
        if (string.IsNullOrWhiteSpace(first))
        {
            return string.IsNullOrWhiteSpace(second) ? "" : second;
        }

        return string.IsNullOrWhiteSpace(second) ? first : $"{first} {second}";
    }

    public static string FormatNumericWithNoun(string format, uint number, string form1, string form24,
        string formAlot)
    {
        string form = GetNounForm(number, form1, form24, formAlot);
        return format.Format(number, form);
    }

    public static string GetNounForm(uint number, string form1, string form24, string formAlot)
    {
        if (number % 100 is >= 11 and <= 14)
        {
            return formAlot;
        }

        return (number % 10) switch
        {
            1 => form1,
            >= 2 and <= 4 => form24,
            _ => formAlot
        };
    }
}