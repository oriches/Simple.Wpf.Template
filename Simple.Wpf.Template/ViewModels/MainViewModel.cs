using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using NLog;
using Simple.Wpf.Template.Commands;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.Services;
using Simple.Wpf.Template.Services.Notifications;

namespace Simple.Wpf.Template.ViewModels;

[UsedImplicitly]
public sealed class MainViewModel : DisposableViewModel, IMainViewModel, IRegisteredViewModel
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly Func<ISettingsViewModel> _settingsFunc;

    private ISettingsViewModel _settings;

    public MainViewModel(Func<ISettingsViewModel> settingsFunc, INotificationService notificationService,
        ISchedulers schedulers)
    {
        _settingsFunc = settingsFunc;
        
        var cancellationTokenSource = new CancellationTokenSource();
        Disposable.Create(() => cancellationTokenSource.Cancel())
            .DisposeWith(this);

        ThrowFromUiThreadCommand = ReactiveCommand<string>.Create()
            .DisposeWith(this);

        ThrowFromTaskCommand = ReactiveCommand<string>.Create()
            .DisposeWith(this);

        ThrowFromRxCommand = ReactiveCommand<string>.Create()
            .DisposeWith(this);

        SimpleNotificationCommand = ReactiveCommand<string>.Create()
            .DisposeWith(this);

        SnoozeNotificationCommand = ReactiveCommand<string>.Create()
            .DisposeWith(this);

        ThrowFromUiThreadCommand
            .ActivateGestures()
            .Subscribe(x =>
            {
                Logger.Info($"{nameof(ThrowFromUiThreadCommand)} executing...");
                schedulers.Dispatcher.Schedule(() => throw new Exception(x + " - UI thread"));
            })
            .DisposeWith(this);

        ThrowFromTaskCommand
            .ActivateGestures()
            .Subscribe(x =>
            {
                Logger.Info($"{nameof(ThrowFromTaskCommand)} executing...");
                Task.FromException(new Exception(x + " - Task"));
            })
            .DisposeWith(this);

        ThrowFromRxCommand
            .ActivateGestures()
            .Subscribe(x =>
            {
                Logger.Info($"{nameof(ThrowFromRxCommand)} executing...");
                Observable.Start(() => throw new Exception(x + " - Rx"), schedulers.TaskPool)
                    .Subscribe()
                    .DisposeWith(this);
            })
            .DisposeWith(this);

        SimpleNotificationCommand.Subscribe(text =>
                notificationService.ExecuteAsync(NotificationType.Message, new object[] { text }, cancellationTokenSource.Token))
            .DisposeWith(this);

        SnoozeNotificationCommand.Subscribe(text =>
                notificationService.ExecuteAsync(NotificationType.MessageWithSnooze, new object[] { text }, cancellationTokenSource.Token))
            .DisposeWith(this);
    }

    public IReactiveCommand<string> ThrowFromUiThreadCommand { get; }

    public IReactiveCommand<string> ThrowFromTaskCommand { get; }

    public IReactiveCommand<string> ThrowFromRxCommand { get; }

    public IReactiveCommand<string> SimpleNotificationCommand { get; }

    public IReactiveCommand<string> SnoozeNotificationCommand { get; }

    public ISettingsViewModel Settings =>
        _settings ??= _settingsFunc()
            .DisposeWith(this);
}