namespace Simple.Wpf.Template.Services
{
    using System;
    using Models;

    public interface IDiagnosticsService
    {
        IObservable<string> Log { get; }
            
        IObservable<Memory> Memory { get; }

        IObservable<int> Cpu { get; }

        IObservable<int> Rps { get; }
    }
}
