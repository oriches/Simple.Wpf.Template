namespace WpfTemplate
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;
    using Extensions;
    using NLog;
    using Services;
    using Views;

    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;

        public App()
        {
#if DEBUG
            LogManager.Configuration = new NLog.Config.XmlLoggingConfiguration("NLog.Debug.config");
#endif
            _disposable = new CompositeDisposable();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            Logger.Info("Starting");
            Logger.Info("Dispatcher managed thread identifier - {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            base.OnStartup(e);

            BootStrapper.Start();

            var viewModel = BootStrapper.RootVisual;
            var window = new MainWindow { DataContext = viewModel };

            window.Show();

            window.Closed += (s, a) =>
            {

                // Performance counters can make a process hang when exiting if they haven't finished
                // initialising, put in this hack to make sure they've initialised.
                BootStrapper.Resolve<IDiagnosticsService>().Initialised.Wait();

                _disposable.Dispose();
                BootStrapper.Stop();
            };

            Current.Exit += (s, a) =>
            {
                Logger.Info("Bye Bye!");
                LogManager.Flush();
            };

            window.Show();
            
            if (Logger.IsInfoEnabled)
            {
                var dianosticsService = BootStrapper.Resolve<IDiagnosticsService>();
                var heartBeat = BootStrapper.Resolve<Heartbeat>();

                _disposable.Add(heartBeat.Listen
                    .SelectMany(x => dianosticsService.Memory.Take(1), (x, y) => y)
                    .Subscribe(x => Logger.Info("Heartbeat, total memory - {0}", x.WorkingSetPrivateAsString())));
            }
#if DEBUG
            _disposable.Add(ObserveUiFreeze());
#endif
            Logger.Info("Started");
        }
       
        private static IDisposable ObserveUiFreeze()
        {
            var watermark = Constants.UiFreeze.TotalMilliseconds;
            var previous = DateTime.Now;

            return BootStrapper.Resolve<IIdleService>()
                .Idling()
                .Select(x =>
                {
                    var current = DateTime.Now;
                    var delta = current - previous;

                    previous = current;

                    return delta;
                })
                .Where(x => x.TotalMilliseconds > watermark)
                .Subscribe(x => Debug.WriteLine(string.Format("UI Freeze = {0} ms", x.TotalMilliseconds)));
        }
    }
}
