using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using ControlzEx.Theming;
using MahApps.Metro.Controls.Dialogs;
using NLog;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Views;

public partial class MainWindow
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static readonly MetroDialogSettings DialogSettings = new()
    {
        AnimateShow = true,
        AnimateHide = true,
        ColorScheme = MetroDialogColorScheme.Accented
    };

    private readonly CompositeDisposable _disposable;

    private readonly ISchedulers _schedulers;

    public MainWindow(IDialogService dialogService, ISchedulers schedulers)
    {
        _schedulers = schedulers;
        _disposable = new CompositeDisposable();

        InitializeComponent();

        Closed += HandleClosed;
        Loaded += HandleLoaded;
        PreviewKeyDown += HandlePreviewKeyDown;

        DialogSettings.CancellationToken = new CancellationTokenSource().DisposeWith(_disposable)
            .Token;

        dialogService.Show.Where(content => content != null)
            .Delay(Constants.ExceptionDelay, schedulers.Dispatcher)
            .Select(content => new MessageDialog(content))
            .SelectMany(ShowDialogAsync)
            .Subscribe()
            .DisposeWith(_disposable);
    }

    private void HandleClosed(object sender, EventArgs args) => _disposable.Dispose();

    private static void HandleLoaded(object sender, RoutedEventArgs args) => UpdateTheme();

    private static void UpdateTheme()
    {
        var brush = (Brush)Application.Current.Resources[ResourceKeys.Brushes.Accent];
        var color = (Color)Application.Current.Resources[ResourceKeys.Colors.Accent];
        ThemeManager.Current.AddTheme(new Theme(Constants.ThemeName, Constants.ThemeName, Constants.ThemeAccent,
            Constants.ColorScheme, color, brush, true, false));

        ThemeManager.Current.ChangeTheme(Application.Current, Constants.ThemeName);
        Application.Current?.MainWindow?.Activate();
    }

    private void HandlePreviewKeyDown(object sender, KeyEventArgs args)
    {
        if (args.Key == Key.Escape && Keyboard.Modifiers == ModifierKeys.None)
            if (IsAnyDialogOpen)
                this.GetCurrentDialogAsync<MessageDialog>()
                    .ToObservable(_schedulers.Dispatcher)
                    .Take(1)
                    .Subscribe(dialog => dialog.CloseableContent.Cancel())
                    .DisposeWith(_disposable);
    }

    private IObservable<Unit> ShowDialogAsync(MessageDialog dialog)
    {
        Logger.Info(() => $"Showing Dialog, Type=[{dialog.CloseableContent.GetType()}]");

        return this.ShowMetroDialogAsync(dialog, DialogSettings)
            .ToObservable(_schedulers.Dispatcher)
            .SelectMany(_ => dialog.CloseableContent.Closed)
            .SelectMany(_ =>
            {
                Logger.Info(() => $"Showing Dialog, Type=[{dialog.CloseableContent.GetType()}]");
                return this.HideMetroDialogAsync(dialog)
                    .ToObservable(_schedulers.Dispatcher);
            })
            .Take(1);
    }
}