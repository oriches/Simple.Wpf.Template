namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;
    using Models;

    public interface IDiagnosticsService
    {
        IObservable<Unit> Initialised { get; }

        IObservable<Memory> Memory { get; }

        IObservable<int> CpuUtilisation { get; }
    }
}
