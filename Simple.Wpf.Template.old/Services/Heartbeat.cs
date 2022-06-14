namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Concurrency;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using NLog;

    public sealed class Heartbeat : IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectableObservable<Unit> _listen;
        private readonly IDisposable _disposable;

        public Heartbeat() : this(Constants.Heartbeat)
        {
        }

        public Heartbeat(TimeSpan interval)
        {
            _listen = Observable.Interval(interval, TaskPoolScheduler.Default)
                .Select(x => Unit.Default)
                .Publish();

            _disposable = _listen.Connect();
        }
        
        public IObservable<Unit> Listen => _listen;

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }
    }
}
