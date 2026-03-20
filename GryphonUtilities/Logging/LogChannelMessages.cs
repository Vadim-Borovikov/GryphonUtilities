using GryphonUtilities.Time;
using JetBrains.Annotations;

namespace GryphonUtilities.Logging;

[PublicAPI]
public class LogChannelMessages : LogChannel
{
    public override string FilePath => Path.Combine(MessageLogDirectory, MessageLogNameToday);
    public override TimeFormat DefaultTimeFormat => TimeFormat.Time;
    public override bool DefaultTimeOnSameLine => true;

    public LogChannelMessages(Clock clock) : base(clock) { }

    protected  override void Log(string message, TimeFormat? timeFormat = null, bool? timeOnSameLine = null,
        CallerInfo? callerInfo = null)
    {
        RenameLogFileIfNeeded();

        base.Log(message, timeFormat, timeOnSameLine, callerInfo);
    }

    private void RenameLogFileIfNeeded()
    {
        lock (Locker)
        {
            if (!File.Exists(FilePath))
            {
                return;
            }

            DateTime modifiedUtc = File.GetLastWriteTimeUtc(FilePath);
            DateTimeFull modified = new(modifiedUtc, Clock.TimeZoneInfo);
            if (modified.DateOnly < Clock.Now().DateOnly)
            {
                string newPath = GetMessageLogPathFor(modified.DateOnly);
                File.Move(FilePath, newPath);
            }
        }
    }

    public void DeleteOldLogs()
    {
        lock (Locker)
        {
            HashSet<string> newLogs = new();
            for (byte days = 0; days < LogsToHold; ++days)
            {
                DateOnly date = Clock.Now().DateOnly.AddDays(-days);
                string name = GetMessageLogPathFor(date);
                newLogs.Add(name);
            }

            if (!Directory.Exists(MessageLogDirectory))
            {
                return;
            }
            List<string> oldLogs =
                Directory.EnumerateFiles(MessageLogDirectory).Where(f => !newLogs.Contains(f)).ToList();
            foreach (string log in oldLogs)
            {
                File.Delete(log);
            }
        }
    }

    private string GetMessageLogPathFor(DateOnly day)
    {
        return day == Clock.Now().DateOnly ? FilePath : Path.Combine(MessageLogDirectory, $"{day:yyyy.MM.dd}.txt");
    }

    private const string MessageLogDirectory = "Logs";
    private const string MessageLogNameToday = "today.txt";

    private const byte LogsToHold = 5;
}