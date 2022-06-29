using System;
using System.Reactive.Concurrency;
using System.Threading;
using JetBrains.Annotations;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class Schedulers : ISchedulers, IRegisteredService
{
    public Schedulers()
    {
        if (SynchronizationContext.Current == null)
            throw new Exception("No SynchronizationContext for thread, needs to be created on the main UI thread.");

        Dispatcher = new SynchronizationContextScheduler(SynchronizationContext.Current);
    }

    public IScheduler Dispatcher { get; }

    public IScheduler TaskPool => TaskPoolScheduler.Default;

    public IScheduler NamedThread(string name)
    {
        return new EventLoopScheduler(threadStart => new Thread(threadStart) { Name = name, IsBackground = true });
    }
}