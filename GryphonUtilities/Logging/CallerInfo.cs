namespace GryphonUtilities.Logging;

public readonly struct CallerInfo
{
    internal CallerInfo(string memberName, string filePath, int lineNumber)
    {
        _memberName = memberName;
        _filePath = filePath;
        _lineNumber = lineNumber;
    }

    public override string ToString() => $"at {_memberName} in {_filePath}:line {_lineNumber}";

    private readonly string _memberName;
    private readonly string _filePath;
    private readonly int _lineNumber;
}