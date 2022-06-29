using System;
using System.Reactive.Linq;
using System.Windows;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PostLaunch, Position = 2)]
public sealed class AutoShutdownModule : BaseModule
{
    private static readonly TimeSpan ShutdownTime = new(23, 45, 0);
    private readonly IDateTimeService _dateTimeService;

    public AutoShutdownModule()
    {
        Logger.Info(() => "Begin");

        var configurationService = Bootstrapper.Resolve<IConfigurationService>();
        if (configurationService.EnableAutoShutdown)
        {
            _dateTimeService = Bootstrapper.Resolve<IDateTimeService>();

            var schedulers = Bootstrapper.Resolve<ISchedulers>();

            var runAt = GetLocalDateTime(ShutdownTime);
            var delay = runAt - _dateTimeService.LocalDateTime;

            Observable.Timer(delay)
                .ObserveOn(schedulers.Dispatcher)
                .Subscribe(_ =>
                {
                    Logger.Info(() => "Automatic Shutdown Started");
                    Application.Current.Shutdown();
                })
                .DisposeWith(this);

            Logger.Info(() => "Automatic Shutdown Register, Time=[{runAt}]");
        }
        else
        {
            Logger.Info(() => "Automatic Shutdown Disabled");
        }

        Logger.Info(() => "End");
    }

    private DateTime GetLocalDateTime(TimeSpan time)
    {
        var date = _dateTimeService.Date;
        var dateTime = DateTime.SpecifyKind(new DateTime(date.Year, date.Month, date.Day, time.Hours, time.Minutes, 0),
            DateTimeKind.Local);

        return dateTime;
    }
}