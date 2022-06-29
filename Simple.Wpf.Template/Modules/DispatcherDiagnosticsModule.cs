using System;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Windows.Threading;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PreLaunch, Position = 999)]
public sealed class DispatcherDiagnosticsModule : BaseModule
{
    private static readonly TimeSpan Interval = TimeSpan.FromMilliseconds(300);
    private static readonly TimeSpan Limit = TimeSpan.FromMilliseconds(600);
    private DateTime _previous;

    public DispatcherDiagnosticsModule()
    {
        Logger.Info(() => "Begin");

        var start = false;
#if DEBUG
        Logger.Info(() => "Mode=[Debug]");
#else
        Logger.Info(() => "Mode=[Release]");
#endif
        if (Debugger.IsAttached) Logger.Info(() => $"Debugger.IsAttached=[{Debugger.IsAttached}]");

        if (start)
        {
            var timer = new DispatcherTimer(DispatcherPriority.Normal)
            {
                Interval = Interval
            };

            _previous = DateTime.Now;
            timer.Tick += HandleTick;

            Disposable.Create(() =>
                {
                    timer.Stop();
                    timer.Tick -= HandleTick;
                })
                .DisposeWith(this);
        }

        Logger.Info(() => "End");
    }

    private void HandleTick(object sender, EventArgs e)
    {
        var current = DateTime.Now;
        var delta = current - _previous;
        _previous = current;

        if (delta > Limit)
        {
            var message = $"UI Freeze=[{delta.TotalMilliseconds:)}ms]";
            Console.WriteLine(message);
            Logger.Info(message);
        }
    }
}