using JetBrains.Annotations;
using Simple.Wpf.Template.Helpers;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Modules;

[UsedImplicitly]
[ModuleConfiguration(Context = ModuleContext.PreLaunch, Position = 1)]
public sealed class BootstrappingModule : BaseModule
{
    public BootstrappingModule()
    {
        Logger.Info(() => "Begin");

        Bootstrapper.Start();

        Bootstrapper.Resolve<IConfigurationService>();

        ApplicationCleanup.RegisterForCleanup(() =>
        {
            Logger.Info(() => "Bootstrapper.StopAsync - Starting");
            return Bootstrapper.StopAsync()
                .ContinueWith(t =>
                {
                    if (t.IsCompleted)
                    {
                        Logger.Info(() => "Bootstrapper.StopAsync - Completed");
                    }
                    else if (t.IsFaulted)
                    {
                        Logger.Error(() => "Bootstrapper.StopAsync - Failed");
                        if (t.Exception != null)
                            Logger.Error(t.Exception.ToString);
                    }
                });
        });

        Logger.Info(() => "End");
    }
}