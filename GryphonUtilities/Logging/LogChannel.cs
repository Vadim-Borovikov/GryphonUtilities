using GryphonUtilities.Time;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace GryphonUtilities.Logging;

[PublicAPI]
public abstract class LogChannel
{
    public enum TimeFormat
    {
        None,
        Time,
        DateTime
    }

    public abstract TimeFormat DefaultTimeFormat { get; }
    public abstract bool DefaultTimeOnSameLine { get; }

    public abstract string FilePath { get; }

    protected readonly object Locker = new();

    protected readonly Clock Clock;

    protected LogChannel(Clock clock) => Clock = clock;

    public void Log(string message, bool includeCallerInfo, TimeFormat? timeFormat = null, bool? timeOnSameLine = null,
        [CallerMemberName] string? callerMemberName = default, [CallerFilePath] string? callerFilePath = default,
        [CallerLineNumber] int callerLineNumber = default)
    {
        CallerInfo? callerInfo =
            includeCallerInfo ? new CallerInfo(callerMemberName ?? "", callerFilePath ?? "", callerLineNumber) : null;
        Log(message, timeFormat, timeOnSameLine, callerInfo);
    }

    protected virtual void Log(string message, TimeFormat? timeFormat = null, bool? timeOnSameLine = null,
        CallerInfo? callerInfo = null)
    {
        timeFormat ??= DefaultTimeFormat;

        string timePrefix = string.Empty;
        if (timeFormat != TimeFormat.None)
        {
            string format = "HH:mm:ss";
            if (timeFormat == TimeFormat.DateTime)
            {
                format = $"dd.MM {format}";
            }
            timePrefix = Clock.Now().ToString(format);
            timeOnSameLine ??= DefaultTimeOnSameLine;
            timePrefix += timeOnSameLine.Value ? ": " : Environment.NewLine;
        }

        if (callerInfo is not null)
        {
            timePrefix += $"{callerInfo}{Environment.NewLine}";
        }

        InsertToStart($"{timePrefix}{message}{Environment.NewLine}");
    }

    private void InsertToStart(string? contents)
    {
        lock (Locker)
        {
            string? directoryPath = Path.GetDirectoryName(FilePath);
            if (!string.IsNullOrWhiteSpace(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            string text = File.Exists(FilePath) ? File.ReadAllText(FilePath) : "";
            File.WriteAllText(FilePath, $"{contents}{text}", Encoding.UTF8);
        }
    }
}