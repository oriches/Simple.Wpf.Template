namespace WpfTemplate.Services
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Disposables;
    using System.Reactive.Linq;
    using System.Reactive.Subjects;
    using Models;
    using NLog;

    public sealed class DiagnosticsService : IDiagnosticsService, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly ISchedulerService _schedulerService;
        private readonly IDisposable _disposable;
        private readonly IConnectableObservable<Counters> _bufferedInactiveObservable;
        private readonly ReplaySubject<Unit> _initialised;
        
        internal sealed class Counters
        {
            public Counters(PerformanceCounter workingSetCounter, PerformanceCounter cpuCounter)
            {
                WorkingSet = workingSetCounter;
                Cpu = cpuCounter;
            }

            public PerformanceCounter WorkingSet { get; private set; }

            public PerformanceCounter Cpu { get; private set; }
        }

        public DiagnosticsService(IIdleService idleService, ISchedulerService schedulerService)
        {
            _schedulerService = schedulerService;

            _initialised = new ReplaySubject<Unit>(1);
            _disposable = new CompositeDisposable();

            _bufferedInactiveObservable = Observable.Create<Counters>(x =>
            {
                var disposable = new CompositeDisposable();

                try
                {
                    var processName = GetProcessInstanceName();

                    Logger.Info("Creating performance counter 'Working Set'");

                    var workingSetCounter = new PerformanceCounter("Process", "Working Set", processName);
                    disposable.Add(workingSetCounter);

                    Logger.Info("Creating performance counter '% Processor Time'");

                    var cpuCounter = new PerformanceCounter("Process", "% Processor Time", processName);
                    disposable.Add(cpuCounter);

                    using (Duration.Measure(Logger, "Initialising created performance counters"))
                    {
                        workingSetCounter.NextValue();
                        cpuCounter.NextValue();
                    }

                    x.OnNext(new Counters(workingSetCounter, cpuCounter));

                    Logger.Info("Ready");
                }
                catch (ArgumentException exn)
                {
                    LogFailToCreatePerformanceCounter(x, exn);
                }
                catch (InvalidOperationException exn)
                {
                    LogFailToCreatePerformanceCounter(x, exn);
                }
                catch (Win32Exception exn)
                {
                    LogFailToCreatePerformanceCounter(x, exn);
                }
                catch (PlatformNotSupportedException exn)
                {
                    LogFailToCreatePerformanceCounter(x, exn);
                }
                catch (UnauthorizedAccessException exn)
                {
                    LogFailToCreatePerformanceCounter(x, exn);
                }
                finally
                {
                    _initialised.OnNext(Unit.Default);
                    _initialised.OnCompleted();
                }

                return disposable;
            })
                .DelaySubscription(Constants.DiagnosticsSubscriptionDelay, schedulerService.TaskPool)
                .SubscribeOn(schedulerService.TaskPool)
                .ObserveOn(schedulerService.TaskPool)
                .CombineLatest(idleService.Idling().Buffer(Constants.DiagnosticsIdleBuffer, schedulerService.TaskPool).Where(x => x.Any()), (x, y) => x)
                .Publish();

            _disposable = _bufferedInactiveObservable.Connect();

            Logger.Info("Ready");
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public IObservable<Unit> Initialised { get { return _initialised; } }

        public IObservable<Memory> Memory
        {
            get
            {
                return _bufferedInactiveObservable.Select(CalculateMemoryValues)
                    .DistinctUntilChanged(WpfTemplate.Memory.Comparer);
            }
        }

        public IObservable<int> CpuUtilisation
        {
            get
            {
                return _bufferedInactiveObservable.Select(CalculateCpu)
                    .Delay(Constants.DiagnosticsCpuBuffer, _schedulerService.TaskPool)
                    .DistinctUntilChanged()
                    .Select(DivideByNumberOfProcessors);
            }
        }

        private static void LogFailToCreatePerformanceCounter(IObserver<Counters> counters, Exception exception)
        {
            Logger.Error("Failed to create performance counters!");
            Logger.Error(exception);
            counters.OnError(exception);
        }

        private static void LogFailToCalculateMemory(Exception exception)
        {
            Logger.Warn("Failed to calculate memory!");
            Logger.Warn(exception);
        }

        private static void LogFailToCalculateCpu(Exception exception)
        {
            Logger.Warn("Failed to calculate cpu!");
            Logger.Warn(exception);
        }

        private Memory CalculateMemoryValues(Counters counters)
        {
            try
            {
                var rawValue = counters.WorkingSet.NextValue();
                var privateWorkingSet = Convert.ToDecimal(rawValue);
                
                var managed = GC.GetTotalMemory(false);

                return new Memory(privateWorkingSet, managed);
            }
            catch (InvalidOperationException exn)
            {
                LogFailToCalculateMemory(exn);
                return new Memory(0, 0);
            }
            catch (Win32Exception exn)
            {
                LogFailToCalculateMemory(exn);
                return new Memory(0, 0);
            }
            catch (PlatformNotSupportedException exn)
            {
                LogFailToCalculateMemory(exn);
                return new Memory(0, 0);
            }
            catch (UnauthorizedAccessException exn)
            {
                LogFailToCalculateMemory(exn);
                return new Memory(0, 0);
            }
            catch (OverflowException exn)
            {
                LogFailToCalculateMemory(exn);
                return new Memory(0, 0);
            }
        }

        private static int CalculateCpu(Counters counters)
        {
            try
            {
                var rawValue = counters.Cpu.NextValue();
                return Convert.ToInt32(rawValue);
            }
            catch (InvalidOperationException exn)
            {
                LogFailToCalculateCpu(exn);
                return 0;
            }
            catch (Win32Exception exn)
            {
                LogFailToCalculateCpu(exn);
                return 0;
            }
            catch (PlatformNotSupportedException exn)
            {
                LogFailToCalculateCpu(exn);
                return 0;
            }
            catch (UnauthorizedAccessException exn)
            {
                LogFailToCalculateCpu(exn);
                return 0;
            }
            catch (OverflowException exn)
            {
                LogFailToCalculateCpu(exn);
                return 0;
            }
        }

        private static int DivideByNumberOfProcessors(int value)
        {
            try
            {
                return value == 0 ? 0 : value / Environment.ProcessorCount;
            }
            catch (OverflowException)
            {
                return 0;
            }
        }

        private static string GetProcessInstanceName()
        {
            var currentProcess = Process.GetCurrentProcess();
            foreach (var instance in new PerformanceCounterCategory("Process").GetInstanceNames()
                .Where(x => x.StartsWith(currentProcess.ProcessName, StringComparison.InvariantCulture)))
            {
                try
                {
                    using (var counter = new PerformanceCounter("Process", "ID Process", instance, true))
                    {
                        var val = (int)counter.RawValue;
                        if (val == currentProcess.Id)
                        {
                            return instance;
                        }
                    }
                }
                catch (ArgumentException)
                {
                }
                catch (InvalidOperationException)
                {
                }
                catch (Win32Exception)
                {
                }
                catch (PlatformNotSupportedException)
                {
                }
                catch (UnauthorizedAccessException)
                {
                }
            }

            throw new ArgumentException(@"Could not find performance counter instance name for current process, name '{0}'", currentProcess.ProcessName);
        }
    }
}
