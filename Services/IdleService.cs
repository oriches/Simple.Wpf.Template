namespace WpfTemplate.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;
    using NLog;

    public sealed class IdleService : IIdleService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectableObservable<EventPattern<EventArgs>> _idleObservable;
        private readonly IDisposable _disposable;

        public IdleService(ISchedulerService schedulerService)
        {
            var mainWindow = Application.Current.MainWindow;

            _idleObservable = Observable.FromEventPattern(h => mainWindow.Dispatcher.Hooks.DispatcherInactive += h,
                h => mainWindow.Dispatcher.Hooks.DispatcherInactive -= h, schedulerService.TaskPool)
                .Publish();

            _disposable = _idleObservable.Connect();
            _disposable = Disposable.Empty;
        }

        public IObservable<Unit> Idling()
        {
            return _idleObservable.Select(x => Unit.Default);
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }
    }
}
