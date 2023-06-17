using JetBrains.Annotations;

namespace GryphonUtilities;

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

    public static string FormatLines(IEnumerable<string?> lines, params object[] args)
    {
        string format = JoinLines(lines);
        return string.Format(format, args);
    }

    public static string FormatNumericWithNoun(string format, uint number, string form1, string form24,
        string formAlot)
    {
        string form = GetNounForm(number, form1, form24, formAlot);
        return string.Format(format, number, form);
    }

    public static string GetNounForm(uint number, string form1, string form24, string formAlot)
    {
        if (number is >= 11 and <= 14)
        {
            return formAlot;
        }

        return (number % 10) switch
        {
            1             => form1,
            >= 2 and <= 4 => form24,
            _             => formAlot
        };
    }

    public static string JoinLines(IEnumerable<string?> lines) => string.Join(Environment.NewLine, lines);
}