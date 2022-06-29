using System.Windows;
using JetBrains.Annotations;
using Simple.Wpf.Template.Services;
using Simple.Wpf.Template.Views;

// ReSharper disable UseObjectOrCollectionInitializer

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.Launch)]
public sealed class MainWindowModule : BaseModule
{
    private readonly MainWindow _window;

    public MainWindowModule()
    {
        Logger.Info(() => "Begin");

        var dialogService = Bootstrapper.Resolve<IDialogService>();
        var schedulers = Bootstrapper.Resolve<ISchedulers>();

        Logger.Info(() => $"Creating Window, Type=[{typeof(MainWindow)}]");

        _window = new MainWindow(dialogService, schedulers);
        _window.DataContext = Bootstrapper.Root;
        _window.IsVisibleChanged += HandleIsVisibleChanged;

        Logger.Info(() => "Created Window");

        // let's go...
        _window.Show();
    }

    private void HandleIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
    {
        if (_window.IsVisible)
        {
            _window.IsVisibleChanged -= HandleIsVisibleChanged;
            Logger.Info(() => "End");
        }
    }
}