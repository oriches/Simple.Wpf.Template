namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using Collections;
    using Extensions;
    using Models;
    using NLog;
    using Services;

    public sealed class DiagnosticsViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly CompositeDisposable _disposable;
        private readonly RangeObservableCollection<string> _log;

        private string _cpu;
        private string _managedMemory;
        private string _totalMemory;
        private string _rps;
        
        internal sealed class FormattedMemory
        {
            public string ManagedMemory { get; }
            public string TotalMemory { get; }

            public FormattedMemory(string managedMemory, string totalMemory)
            {
                ManagedMemory = managedMemory;
                TotalMemory = totalMemory;
            }
        }

        public DiagnosticsViewModel(IDiagnosticsService diagnosticsService, ISchedulerService schedulerService)
        {
            Id = $"Identifier: {Guid.NewGuid()}";

            Rps = Constants.DefaultRpsString;
            Cpu = Constants.DefaultCpuString;
            ManagedMemory = Constants.DefaultManagedMemoryString;
            TotalMemory = Constants.DefaultTotalMemoryString;

            _log = new RangeObservableCollection<string>();
            
            _disposable = new CompositeDisposable
            {
                diagnosticsService.Log
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => _log.Add(x),
                        e =>
                        {
                            Logger.Error(e);
                            _log.Clear();
                        }),

                diagnosticsService.Rps
                    .Select(FormatRps)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => Rps = x,
                        e =>
                        {
                            Logger.Error(e);
                            Rps = Constants.DefaultRpsString;
                        }),

                diagnosticsService.Cpu
                    .Select(FormatCpu)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x => Cpu = x,
                        e =>
                        {
                            Logger.Error(e);
                            Cpu = Constants.DefaultCpuString;
                        }),

                diagnosticsService.Memory
                    .Select(FormatMemory)
                    .ObserveOn(schedulerService.Dispatcher)
                    .Subscribe(x =>
                    {
                        ManagedMemory = x.ManagedMemory;
                        TotalMemory = x.TotalMemory;
                    }, e =>
                    {
                        Logger.Error(e);
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

        public string Id { get; private set; }

        public IEnumerable<string> Log => _log;

        public string Rps
        {
            get
            {
                return _rps;
            }

            private set
            {
                SetPropertyAndNotify(ref _rps, value, () => Rps);
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

        private static string FormatRps(int rps)
        {
            return "Render: " + rps.ToString(CultureInfo.InvariantCulture) + " RPS";
        }

        private static string FormatCpu(int cpu)
        {
            return cpu < 10
                ? $"CPU: 0{cpu.ToString(CultureInfo.InvariantCulture)} %"
                : $"CPU: {cpu.ToString(CultureInfo.InvariantCulture)} %";
        }

        private static FormattedMemory FormatMemory(Memory memory)
        {
            var managedMemory = $"Managed Memory: {memory.ManagedAsString()}";
            var totalMemory = $"Total Memory: {memory.WorkingSetPrivateAsString()}";

            return new FormattedMemory(managedMemory, totalMemory);
        }
    }
}
