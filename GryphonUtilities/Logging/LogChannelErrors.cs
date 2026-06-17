using GryphonUtilities.Extensions;
using GryphonUtilities.Time;
using JetBrains.Annotations;
using System.Runtime.CompilerServices;

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

    public void Log(Exception ex, TimeFormat? timeFormat = null, bool? timeOnSameLine = null)
    {
        string title = ex.Message;
        string body = ex.FlattenAll()
                        .Select(e => e.ToString())
                        .Join($"{Environment.NewLine}{Environment.NewLine}");
        Log(title, body, timeFormat, timeOnSameLine);
    }

    public void Log(string title, string body, bool includeCallerInfo, TimeFormat? timeFormat = null,
        bool? timeOnSameLine = null, [CallerMemberName] string? callerMemberName = default,
        [CallerFilePath] string? callerFilePath = default, [CallerLineNumber] int callerLineNumber = default)
    {
        CallerInfo? callerInfo =
            includeCallerInfo ? new CallerInfo(callerMemberName ?? "", callerFilePath ?? "", callerLineNumber) : null;
        Log(title, body, timeFormat, timeOnSameLine, callerInfo);
    }

    protected override void Log(string message, TimeFormat? timeFormat = null, bool? timeOnSameLine = null,
        CallerInfo? callerInfo = null)
    {
        Log(message, message, timeFormat, timeOnSameLine, callerInfo);
    }

    private void Log(string title, string body, TimeFormat? timeFormat = null, bool? timeOnSameLine = null,
        CallerInfo? callerInfo = null)
    {
        base.Log($"{body}{Environment.NewLine}", timeFormat, timeOnSameLine, callerInfo);
        _onErrorLogged?.Invoke(title);
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