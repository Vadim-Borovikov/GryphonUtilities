using System.Text;
using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public sealed class Logger
{
    public Clock Clock;

    public Logger(Clock clock)
    {
        Clock = clock;
        if (!Directory.Exists(MessagesLogDirectory))
        {
            Directory.CreateDirectory(MessagesLogDirectory);
        }
        DeleteOldLogs();
    }

    public static void DeleteExceptionLog()
    {
        lock (ExceptionsLocker)
        {
            File.Delete(ExceptionsLogPath);
        }
    }

    public void LogStartup()
    {
        LogMessage();
        LogTimedMessage("Startup");
    }

    public void LogMessage(string? message = null)
    {
        if (File.Exists(TodayLogPath))
        {
            DateTime modifiedUtc = File.GetLastWriteTimeUtc(TodayLogPath);
            DateTimeFull modified = new(modifiedUtc, Clock.TimeZoneInfo);
            if (modified.DateOnly < Clock.Now().DateOnly)
            {
                string newPath = GetLogPathFor(modified.DateOnly);
                File.Move(TodayLogPath, newPath);
            }
        }
        InsertToStart(TodayLogPath, $"{message}{Environment.NewLine}", LogsLocker);
    }

    public void LogTimedMessage(string? message = null) => LogMessage($"{Clock.Now():HH:mm:ss}: {message}");

    public void LogError(string message) => LogError(message, message);

    public void LogException(Exception ex)
    {
        string body =
            string.Join($"{Environment.NewLine}{Environment.NewLine}", ex.FlattenAll().Select(e => e.ToString()));
        LogError(ex.Message, body);
    }

    public void LogExceptionIfPresents(Task t)
    {
        if (t.Exception is null)
        {
            return;
        }

        LogException(t.Exception);
    }

    public void LogError(string title, string body)
    {
        LogTimedMessage($"Error: {title}");
        InsertToStart(ExceptionsLogPath,
            $"{Clock.Now():dd.MM HH:mm:ss}{Environment.NewLine}{body}{Environment.NewLine}{Environment.NewLine}",
            ExceptionsLocker);
    }

    private static void InsertToStart(string path, string? contents, object locker)
    {
        lock (locker)
        {
            string text = File.Exists(path) ? File.ReadAllText(path) : "";
            File.WriteAllText(path, $"{contents}{text}", Encoding.UTF8);
        }
    }

    private void DeleteOldLogs()
    {
        lock (LogsLocker)
        {
            HashSet<string> newLogs = new();
            for (byte days = 0; days < LogsToHold; ++days)
            {
                DateOnly date = Clock.Now().DateOnly.AddDays(-days);
                string name = GetLogPathFor(date);
                newLogs.Add(name);
            }

            List<string> oldLogs =
                Directory.EnumerateFiles(MessagesLogDirectory).Where(f => !newLogs.Contains(f)).ToList();
            foreach (string log in oldLogs)
            {
                File.Delete(log);
            }
        }
    }

    private string GetLogPathFor(DateOnly day)
    {
        return day == Clock.Now().DateOnly
            ? TodayLogPath
            : Path.Combine(MessagesLogDirectory, $"{day:yyyy.MM.dd}.txt");
    }

    private static readonly object ExceptionsLocker = new();
    private static readonly object LogsLocker = new();
    private static readonly string TodayLogPath = Path.Combine(MessagesLogDirectory, MessagesLogNameToday);

    private const string ExceptionsLogPath = "errors.txt";
    private const string MessagesLogDirectory = "Logs";
    private const string MessagesLogNameToday = "today.txt";
    private const byte LogsToHold = 5;
}