using System;
using System.Reactive.Concurrency;

namespace Simple.Wpf.Template.Events;

public interface IEventAggregator : IDisposable
{
    void Publish<T>(T @event) where T : BaseEvent;

    IObservable<T> Notify<T>() where T : BaseEvent;

    IObservable<T> Notify<T>(IScheduler scheduler) where T : BaseEvent;
}