using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using JetBrains.Annotations;
using Simple.Wpf.Template.Helpers;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PreLaunch, Position = 2)]
public sealed class ExceptionHandlingModule : BaseModule
{
    public ExceptionHandlingModule()
    {
        Logger.Info(() => "Begin");

        AppDomain.CurrentDomain.UnhandledException += HandleDomainException;
        Application.Current.DispatcherUnhandledException += HandleDispatcherException;
        TaskScheduler.UnobservedTaskException += HandleTaskException;

        Logger.Info(() => "End");
    }

    private static void HandleTaskException(object sender, UnobservedTaskExceptionEventArgs args)
    {
        Logger.Info(() => "Unhandled Task Exception");
        args.SetObserved();

        HandleException(args.Exception.GetBaseException());
    }

    private static void HandleDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs args)
    {
        Logger.Info(() => "Unhandled Dispatcher Exception");
        args.Handled = true;

        HandleException(args.Exception.GetBaseException());
    }

    private static void HandleException(Exception exception) => ExceptionHelper.Handle(exception);

    private static void HandleDomainException(object sender, UnhandledExceptionEventArgs args)
    {
        Logger.Info(() => "Unhandled Domain Exception");
        HandleException(args.ExceptionObject as Exception);
    }
}