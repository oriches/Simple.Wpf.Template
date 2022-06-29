using System;
using System.Reflection;
using System.Threading;
using System.Windows;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PreLaunch, Position = 0)]
public sealed class SingleInstanceModule : BaseModule
{
    public SingleInstanceModule()
    {
        Logger.Info(() => "Begin");

        EnsureSingleInstanceIsRunning()
            .DisposeWith(this);

        Logger.Info(() => "End");
    }

    private static IDisposable EnsureSingleInstanceIsRunning()
    {
        var appName = Assembly.GetExecutingAssembly()
            .FullName;

        var mutex = new Mutex(true, appName, out var createdNew);

        if (!createdNew)
        {
            Logger.Info(() => "Application already started, shutting down this instance...");

            Application.Current.Shutdown();
        }

        return mutex;
    }
}