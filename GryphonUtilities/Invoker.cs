using JetBrains.Annotations;

namespace GryphonUtilities;

[PublicAPI]
public class Invoker : IDisposable
{
    public Invoker(Logger logger)
    {
        _logger = logger;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _cancellationTokenSource.Dispose();
    }

    public void DoAt(Func<CancellationToken, Task> doWork, DateTimeFull at)
    {
        DoAt(doWork, at, _logger, _cancellationTokenSource.Token);
    }

    public void DoAfterDelay(Func<CancellationToken, Task> doWork, TimeSpan delay)
    {
        DoAfterDelay(doWork, delay, _logger, _cancellationTokenSource.Token);
    }

    public void DoPeriodically(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow)
    {
        DoPeriodically(doWork, interval, doNow, _logger, _cancellationTokenSource.Token);
    }

    public void DoPeriodicallySince(Func<CancellationToken, Task> doWork, DateTimeFull start, TimeSpan interval)
    {
        DoPeriodicallySince(doWork, start, interval, _logger, _cancellationTokenSource.Token);
    }

    public static void DoAt(Func<CancellationToken, Task> doWork, DateTimeFull at, Logger logger,
        CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoAtAsync(doWork, at, ct), logger, cancellationToken);
    }

    public static void DoAfterDelay(Func<CancellationToken, Task> doWork, TimeSpan delay, Logger logger,
        CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoAfterDelayAsync(doWork, delay, ct), logger, cancellationToken);
    }

    public static void DoPeriodically(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow,
        Logger logger, CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoPeriodicallyAsync(doWork, interval, doNow, ct), logger, cancellationToken);
    }

    public static void DoPeriodicallySince(Func<CancellationToken, Task> doWork, DateTimeFull start, TimeSpan interval,
        Logger logger, CancellationToken cancellationToken)
    {
        FireAndForget(ct => DoPeriodicallySinceAsync(doWork, start, interval, ct), logger, cancellationToken);
    }

    public static void FireAndForget(Func<CancellationToken, Task> doWork, Logger logger,
        CancellationToken cancellationToken = default)
    {
        Task.Run(() => doWork(cancellationToken), cancellationToken)
            .ContinueWith(logger.LogExceptionIfPresents, cancellationToken);
    }

    private static Task DoAtAsync(Func<CancellationToken, Task> doWork, DateTimeFull at,
        CancellationToken cancellationToken)
    {
        DateTimeFull now = DateTimeFull.CreateNow(at.TimeZoneInfo);
        TimeSpan delay = at - now;
        return DoAfterDelayAsync(doWork, delay, cancellationToken);
    }

    private static async Task DoAfterDelayAsync(Func<CancellationToken, Task> doWork, TimeSpan delay,
        CancellationToken cancellationToken)
    {
        await Task.Delay(delay, cancellationToken);
        await doWork(cancellationToken);
    }

    private static async Task DoPeriodicallyAsync(Func<CancellationToken, Task> doWork, TimeSpan interval, bool doNow,
        CancellationToken cancellationToken)
    {
        if (doNow)
        {
            await doWork(cancellationToken);
        }
        while (!cancellationToken.IsCancellationRequested)
        {
            await DoAfterDelayAsync(doWork, interval, cancellationToken);
        }
    }

    private static Task DoPeriodicallySinceAsync(Func<CancellationToken, Task> doWork, DateTimeFull start,
        TimeSpan interval, CancellationToken cancellationToken)
    {
        return DoAtAsync(ct => DoPeriodicallyAsync(doWork, interval, true, ct), start, cancellationToken);
    }

    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Logger _logger;
}