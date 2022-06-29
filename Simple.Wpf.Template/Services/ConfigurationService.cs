using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using NLog;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Properties;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class ConfigurationService : DisposableService, IConfigurationService, IRegisteredService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly TimeSpan PersistThrottle = TimeSpan.FromSeconds(5);

    private readonly Subject<Unit> _persist;
    private readonly Subject<string> _valueChanged;

    public ConfigurationService(ISchedulers schedulers)
    {
        var scheduler = schedulers.NamedThread("Settings");

        _persist = new Subject<Unit>().DisposeWith(this);
        _valueChanged = new Subject<string>().DisposeWith(this);

        _persist.Throttle(PersistThrottle, scheduler)
            .Subscribe(_ => PersistImpl())
            .DisposeWith(this);

        Disposable.Create(PersistImpl)
            .DisposeWith(this);

        LogManager.GlobalThreshold = LogLevel;
    }

    public bool EnableAutoShutdown
    {
        get => Settings.Default.EnableAutoShutdown;
        set
        {
            if (!Equals(Settings.Default.EnableAutoShutdown, value))
            {
                Settings.Default.EnableAutoShutdown = value;
                _valueChanged.OnNext(nameof(EnableAutoShutdown));
                Persist();
            }
        }
    }

    public LogLevel LogLevel
    {
        get => LogLevel.FromOrdinal(Settings.Default.LogLevel);
        set
        {
            if (!Equals(Settings.Default.LogLevel, value.Ordinal))
            {
                Settings.Default.LogLevel = value.Ordinal;
                Persist();
            }
        }
    }

    private void Persist() => _persist.OnNext(Unit.Default);

    private static void PersistImpl()
    {
        try
        {
            using (Duration.Measure(() => "Persisted User Settings"))
            {
                Settings.Default.Save();
            }
        }
        catch (Exception exception)
        {
            Logger.Error(() => "Failed to persist User Settings...");
            Logger.Error(exception.ToString);
        }
    }
}