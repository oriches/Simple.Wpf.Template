namespace WpfTemplate
{
    using System;
    using System.Reactive;

    public interface IIdleService
    {
        IObservable<Unit> Idling();
    }
}
