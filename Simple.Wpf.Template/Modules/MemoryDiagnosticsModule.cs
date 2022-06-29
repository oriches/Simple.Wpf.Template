using System;
using System.Diagnostics;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PostLaunch, Position = 1)]
public sealed class MemoryDiagnosticsModule : BaseModule
{
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(1);
    private string _processName;

    public MemoryDiagnosticsModule()
    {
        Logger.Info(() => "Begin");
        Logger.Info(() => $"Interval=[{Interval}]");

        var schedulers = Bootstrapper.Resolve<ISchedulers>();
        Observable.Interval(Interval, schedulers.TaskPool)
            .StartWith(0)
            .ObserveOn(schedulers.TaskPool)
            .Subscribe(counter =>
            {
                if (counter == 0) _processName = GetProcessName();

                var privateBytes = ConvertToString(Process.GetCurrentProcess()
                    .PagedMemorySize64);
                var workingSet = ConvertToString(Process.GetCurrentProcess()
                    .WorkingSet64);

                if (_processName != null)
                {
                    if (TryGetCurrentCpuUsage(_processName, out var cpu))
                        Logger.Info(() => $"PrivateBytes=[{privateBytes}], WorkingSet=[{workingSet}], CPU=[{cpu}]");
                }
                else
                {
                    Logger.Info(() => $"PrivateBytes=[{privateBytes}], WorkingSet=[{workingSet}]");
                }
            })
            .DisposeWith(this);
    }

    private static string ConvertToString(long value)
    {
        var valueAsKb = value / 1024;
        return $"{valueAsKb:N0}Kb";
    }

    private static string GetProcessName()
    {
        var process = Process.GetCurrentProcess();
        foreach (var instance in new PerformanceCounterCategory("Process").GetInstanceNames())
            if (instance.StartsWith(process.ProcessName))
            {
                using var processId = new PerformanceCounter("Process", "ID Process", instance, true);
                if (process.Id == (int)processId.RawValue) return instance;
            }

        return null;
    }

    private static bool TryGetCurrentCpuUsage(string processName, out string cpuUsage)
    {
        try
        {
            var cpu = new PerformanceCounter("Process", "% Processor Time", processName, true);
            var value = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
            cpuUsage = value.ToString("00:00");
            return true;
        }
        catch (Exception exception)
        {
            Logger.Warn(() => "Failed to get CPU usage");
            Logger.Warn(exception.ToString);

            cpuUsage = null;
            return false;
        }
    }
}