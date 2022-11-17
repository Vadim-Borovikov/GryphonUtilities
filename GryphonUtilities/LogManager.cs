using System.Text;
using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public sealed class LogManager
{
    public LogManager()
    {
        _timeManager = new TimeManager();
        if (!Directory.Exists(MessagesLogDirectory))
        {
            Directory.CreateDirectory(MessagesLogDirectory);
        }
        _todayLogPath = Path.Combine(MessagesLogDirectory, MessagesLogNameToday);
        DeleteOldLogs();
    }

    public void SetTimeZone(string? timeZoneId = null) => _timeManager = new TimeManager(timeZoneId);

    public void LogMessage(string? message = null)
    {
        if (File.Exists(_todayLogPath))
        {
            DateTime modifiedUtc = File.GetLastWriteTimeUtc(_todayLogPath);
            DateTimeOffset modified = _timeManager.ToLocal(modifiedUtc);
            if (modified.Date < _timeManager.Now().Date)
            {
                string newPath = GetLogPathFor(modified.DateOnly());
                File.Move(_todayLogPath, newPath);
            }
        }
        InsertToStart(_todayLogPath, $"{message}{Environment.NewLine}", _logsLocker);
    }

    public void LogTimedMessage(string? message = null) => LogMessage($"{_timeManager.Now():HH:mm:ss}: {message}");

    public void LogError(string message) => LogError(message, message);

    public void LogException(Exception ex)
    {
        string body =
            string.Join($"{Environment.NewLine}{Environment.NewLine}", Flatten(ex).Select(e => e.ToString()));
        LogError(ex.Message, body);
    }

    public void DeleteExceptionLog()
    {
        lock (_exceptionsLocker)
        {
            File.Delete(ExceptionsLogPath);
        }
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
            $"{_timeManager.Now():dd.MM HH:mm:ss}{Environment.NewLine}{body}{Environment.NewLine}{Environment.NewLine}",
            _exceptionsLocker);
    }

    private void InsertToStart(string path, string? contents, object locker)
    {
        lock (locker)
        {
            string text = File.Exists(path) ? File.ReadAllText(path) : "";
            File.WriteAllText(path, $"{contents}{text}", Encoding.UTF8);
        }
    }

    private void DeleteOldLogs()
    {
        lock (_logsLocker)
        {
            HashSet<string> newLogs = new();
            for (byte days = 0; days < LogsToHold; ++days)
            {
                DateOnly date = _timeManager.Now().DateOnly().AddDays(-days);
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
        return day == _timeManager.Now().DateOnly()
            ? _todayLogPath
            : Path.Combine(MessagesLogDirectory, $"{day:yyyy.MM.dd}.txt");
    }

    private static IEnumerable<Exception> Flatten(Exception ex)
    {
        if (ex is AggregateException aggregateException)
        {
            foreach (Exception e in aggregateException.Flatten().InnerExceptions)
            {
                yield return e;
            }
            yield break;
        }

        yield return ex;
        while (ex.InnerException is not null)
        {
            ex = ex.InnerException;
            yield return ex;
        }
    }

    private TimeManager _timeManager;

    private readonly object _exceptionsLocker = new();
    private readonly object _logsLocker = new();
    private readonly string _todayLogPath;

    private const string ExceptionsLogPath = "errors.txt";
    private const string MessagesLogDirectory = "Logs";
    private const string MessagesLogNameToday = "today.txt";
    private const byte LogsToHold = 5;
}
