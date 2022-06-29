using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using NLog;

namespace Simple.Wpf.Template.Services;

public static class Duration
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static IDisposable Measure(Func<string> message)
    {
        var stopwatch = new Stopwatch();
        stopwatch.Start();

        return Disposable.Create(() =>
        {
            stopwatch.Stop();
            Logger.Info(() =>
                $"{message()}, Ticks=[{stopwatch.ElapsedTicks}], Milliseconds=[{stopwatch.ElapsedMilliseconds}]");
        });
    }
}