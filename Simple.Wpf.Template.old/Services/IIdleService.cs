namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;

    public interface IIdleService
    {
        IObservable<Unit> Idling { get; }
    }
}
