using System;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Simple.Wpf.Template.Extensions;

public static class SchedulerExtensions
{
    public static IDisposable Schedule(this IScheduler scheduler, TimeSpan timeSpan, Action action)
    {
        return scheduler.Schedule(action, timeSpan, (_, s) =>
        {
            s();
            return Disposable.Empty;
        });
    }

    public static IDisposable Schedule(this IScheduler scheduler, Action action)
    {
        return scheduler.Schedule(action, (_, s) =>
        {
            s();
            return Disposable.Empty;
        });
    }
}