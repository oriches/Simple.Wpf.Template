namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Globalization;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Extensions;
    using Models;
    using NLog;
    using Services;

    public sealed class DiagnosticsViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;

        private string _cpu;
        private string _managedMemory;
        private string _totalMemory;
        private string _fps;

        internal sealed class FormattedMemory
        {
            public string ManagedMemory { get; private set; }
            public string TotalMemory { get; private set; }

            public FormattedMemory(string managedMemory, string totalMemory)
            {
                ManagedMemory = managedMemory;
                TotalMemory = totalMemory;
            }
        }

        public DiagnosticsViewModel(IDiagnosticsService diagnosticsService, ISchedulerService schedulerService)
        {
            Fps = Constants.DefaultFpsString;
            Cpu = Constants.DefaultCpuString;
            ManagedMemory = Constants.DefaultManagedMemoryString;
            TotalMemory = Constants.DefaultTotalMemoryString;

            _disposable = new CompositeDisposable
            {
                diagnosticsService.Fps
                    .Sample(TimeSpan.FromMilliseconds(50), schedulerService.TaskPool)
                    .DistinctUntilChanged()
                    .Select(FormatFps)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => { Fps = x; },
                               e => { Fps = Constants.DefaultFpsString; }),

                diagnosticsService.Cpu
                    .Select(FormatCpu)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => { Cpu = x; },
                               e => { Cpu = Constants.DefaultCpuString; }),

                diagnosticsService.Memory
                    .Select(FormatMemory)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x =>
                    {
                        ManagedMemory = x.ManagedMemory;
                        TotalMemory = x.TotalMemory;
                    }, e =>
                    {
                        ManagedMemory = Constants.DefaultManagedMemoryString;
                        TotalMemory = Constants.DefaultTotalMemoryString;
                    })
            };
        }
        
        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public string Fps
        {
            get
            {
                return _fps;
            }

            private set
            {
                SetPropertyAndNotify(ref _fps, value, () => Fps);
            }
        }

        public string Cpu
        {
            get
            {
                return _cpu;
            }

            private set
            {
                SetPropertyAndNotify(ref _cpu, value, () => Cpu);
            }
        }

        public string ManagedMemory
        {
            get
            {
                return _managedMemory;
            }

            private set
            {
                SetPropertyAndNotify(ref _managedMemory, value, () => ManagedMemory);
            }
        }

        public string TotalMemory
        {
            get
            {
                return _totalMemory;
            }

            private set
            {
                SetPropertyAndNotify(ref _totalMemory, value, () => TotalMemory);
            }
        }

        private static string FormatFps(int fps)
        {
            return string.Format("Render: {0} FPS", fps.ToString(CultureInfo.InvariantCulture));
        }

        private static string FormatCpu(int cpu)
        {
            return cpu < 10
                ? string.Format("CPU: 0{0} %", cpu.ToString(CultureInfo.InvariantCulture))
                : string.Format("CPU: {0} %", cpu.ToString(CultureInfo.InvariantCulture));
        }

        private static FormattedMemory FormatMemory(Memory memory)
        {
            {
                var managedMemory = string.Format("Managed Memory: {0}", memory.ManagedAsString());
                var totalMemory = string.Format("Total Memory: {0}", memory.WorkingSetPrivateAsString());

                return new FormattedMemory(managedMemory, totalMemory);
            }
        }
    }
}
