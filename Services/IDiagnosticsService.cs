namespace WpfTemplate
{
    using System;
    using System.Reactive;

    public interface IDiagnosticsService
    {
        IObservable<Unit> Initialised { get; }

        IObservable<Memory> Memory { get; }

        IObservable<int> CpuUtilisation { get; }
    }
}
