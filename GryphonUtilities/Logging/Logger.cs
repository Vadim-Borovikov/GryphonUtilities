using GryphonUtilities.Time;
using JetBrains.Annotations;

namespace GryphonUtilities.Logging;

[PublicAPI]
public class Logger
{
    public Clock Clock;

    public Logger(Clock clock)
    {
        Clock = clock;

        Messages = new LogChannelMessages(Clock);
        Errors = new LogChannelErrors(Clock, title => Messages.Log($"Error: {title}", false));
    }

    public void LogStartup()
    {
        Messages.DeleteOldLogs();

        Messages.Log(string.Empty, false, LogChannel.TimeFormat.None);
        Messages.Log("Startup", false);
    }

    public readonly LogChannelMessages Messages;
    public readonly LogChannelErrors Errors;
}