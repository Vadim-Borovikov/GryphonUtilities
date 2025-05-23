using System.Text;
using GryphonUtilities.Time;
using JetBrains.Annotations;

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

    public virtual void Log(string message, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
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