namespace Simple.Wpf.Template
{
    using System;
    using System.Diagnostics;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Windows;
    using System.Windows.Media;
    using System.Windows.Threading;
    using Extensions;
    using Models;
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
            Logger.Info("Dispatcher managed thread identifier = {0}", System.Threading.Thread.CurrentThread.ManagedThreadId);

            Logger.Info("WPF rendering capability (tier) = {0}", RenderCapability.Tier / 0x10000);
            RenderCapability.TierChanged += (sender, args) => Logger.Info("WPF rendering capability (tier) = {0}", RenderCapability.Tier / 0x10000);

            base.OnStartup(e);

            BootStrapper.Start();

            var window = new MainWindow();

            // The window has to be created before the root visual - all to do with the idling service initialising correctly...
            window.DataContext = BootStrapper.RootVisual;

            window.Closed += (s, a) =>
            {
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
                var heartbeat = new Heartbeat();

                var listenDisposable = heartbeat.Listen
                    .SelectMany(x => dianosticsService.Memory.Take(1), (x, y) => y)
                    .SelectMany(x => dianosticsService.Cpu.Take(1), (x, y) => new Tuple<Memory, int>(x, y))
                    .SelectMany(x => dianosticsService.Fps.Take(1), (x, y) => new Tuple<Memory, int, int>(x.Item1, x.Item2, y))
                    .Subscribe(x => Logger.Info("Heartbeat (Memory={0}, CPU={1}%, FPS={2})", x.Item1.WorkingSetPrivateAsString(), x.Item2, x.Item3));

                _disposable.Add(listenDisposable);
                _disposable.Add(heartbeat);
            }

#if DEBUG
            _disposable.Add(ObserveUiFreeze());
#endif
            Logger.Info("Started");
        }
       
        private static IDisposable ObserveUiFreeze()
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = Constants.UiFreezeTimer
            };

            var previous = DateTime.Now;
            timer.Tick += (sender, args) =>
            {
                var current = DateTime.Now;
                var delta = current - previous;
                previous = current;

                if (delta > Constants.UiFreeze)
                {
                    Debug.WriteLine("UI Freeze = {0} ms", delta.TotalMilliseconds);
                }
            };

            timer.Start();
            return Disposable.Create(timer.Stop);}
    }
}
