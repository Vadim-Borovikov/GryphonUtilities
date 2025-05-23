using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using JetBrains.Annotations;

namespace GryphonUtilities.Logging;

[PublicAPI]
public class LogChannelErrors : LogChannel
{
    public override string FilePath => "errors.txt";
    public override TimeFormat DefaultTimeFormat => TimeFormat.DateTime;
    public override bool DefaultTimeOnSameLine => false;

    public LogChannelErrors(Clock clock, Action<string>? onErrorLogged = null)
        : base(clock)
    {
        _onErrorLogged = onErrorLogged;
    }

    public override void Log(string message, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
    {
        Log(message, message, timeFormat, timeOnSameLine);
    }

    public void Log(string title, string body, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
    {
        base.Log($"{body}{Environment.NewLine}", timeFormat, timeOnSameLine);
        _onErrorLogged?.Invoke(title);
    }

    public void Log(Exception ex, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
    {
        string title = ex.Message;
        string body =
            string.Join($"{Environment.NewLine}{Environment.NewLine}", ex.FlattenAll().Select(e => e.ToString()));
        Log(title, body, timeFormat, timeOnSameLine);
    }

    public void LogExceptionIfPresents(Task task, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
    {
        if (task.Exception is null)
        {
            return;
        }

        Log(task.Exception, timeFormat, timeOnSameLine);
    }

    public void DeleteLog()
    {
        lock (Locker)
        {
            File.Delete(FilePath);
        }
    }

    private readonly Action<string>? _onErrorLogged;
}