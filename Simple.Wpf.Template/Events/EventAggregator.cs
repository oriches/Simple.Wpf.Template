using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Events;

[UsedImplicitly]
public sealed class EventAggregator : IEventAggregator
{
    private readonly Subject<BaseEvent> _eventStream;
    private readonly ISchedulers _schedulers;

    public EventAggregator(ISchedulers schedulers)
    {
        _schedulers = schedulers;
        _eventStream = new Subject<BaseEvent>();
    }

    public void Dispose() => _eventStream.Dispose();

    public void Publish<T>(T @event) where T : BaseEvent => _eventStream.OnNext(@event);

    public IObservable<T> Notify<T>() where T : BaseEvent => Notify<T>(_schedulers.TaskPool);

    public IObservable<T> Notify<T>(IScheduler scheduler) where T : BaseEvent =>
        _eventStream.OfType<T>()
            .ObserveOn(scheduler);
}