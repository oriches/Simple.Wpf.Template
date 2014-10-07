namespace Simple.Wpf.Template.Services
{
    using System;
    using Models;

    public interface IDiagnosticsService
    {
        IObservable<Memory> Memory { get; }

        IObservable<int> Cpu { get; }

        IObservable<int> Fps { get; }
    }
}
