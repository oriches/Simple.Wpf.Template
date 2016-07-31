namespace Simple.Wpf.Template.Services
{
    using System;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using System.Windows;
    using Extensions;
    using NLog;
    using Duration = Services.Duration;

    public sealed class IdleService : IIdleService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IConnectableObservable<EventPattern<object>> _idleObservable;
        private readonly IDisposable _disposable;

        public IdleService(ISchedulerService schedulerService)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow == null)
            {
                throw new Exception("Main window has not been created yet!");
            }

            _idleObservable = Observable.FromEventPattern(h => mainWindow.Dispatcher.Hooks.DispatcherInactive += h,
                h => mainWindow.Dispatcher.Hooks.DispatcherInactive -= h, schedulerService.TaskPool)
                .Publish();

            _disposable = _idleObservable.Connect();
            _disposable = Disposable.Empty;
        }

        public IObservable<Unit> Idling => _idleObservable.AsUnit();

        public void Dispose()
        {
            using (Services.Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }
    }
}
