using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Core;
using NLog;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Helpers;
using Simple.Wpf.Template.Services;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Modules;

public static class Bootstrapper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static IMainViewModel _root;
    private static IContainer _rootScope;

    public static IMainViewModel Root
    {
        get
        {
            if (_rootScope == null)
                Start();

            if (_root != null)
                return _root;

            _root = Resolve<IMainViewModel>();
            return _root;
        }
    }

    public static T Resolve<T>() where T : class
    {
        Logger.Info(() => $"Resolving, Type=[{typeof(T)}]");
        var instance = _rootScope?.Resolve<T>();
        Logger.Info(() => $"Resolved, Type=[{typeof(T)}]");

        return instance;
    }

    public static T Resolve<T>(Parameter[] parameters) where T : class
    {
        Logger.Info(() => $"Resolving, Type=[{typeof(T)}]");
        var instance = _rootScope?.Resolve<T>(parameters);
        Logger.Info(() => $"Resolved, Type=[{typeof(T)}]");

        return instance;
    }

    public static void Start()
    {
        if (_rootScope != null)
            return;

        Logger.Info(() => "Starting");

        var builder = new ContainerBuilder();

        var assemblies = new[] { Assembly.GetExecutingAssembly(), typeof(IRegisteredService).Assembly };

        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => typeof(IRegisteredService).IsAssignableFrom(t))
            .SingleInstance()
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(assemblies)
            .Where(t => typeof(IRegisteredViewModel).IsAssignableFrom(t))
            .AsImplementedInterfaces();

        _rootScope = builder.Build();

        ObservableExtensions.Initialize(Resolve<IGestureService>());

        ExceptionHelper.Initialize(Resolve<IDialogService>(), Resolve<ISchedulers>(), exception =>
        {
            var parameters = new Parameter[] { new NamedParameter(nameof(exception), exception) };
            return _rootScope.Resolve<IExceptionViewModel>(parameters);
        });

        Logger.Info(() => "Completed");
    }

    public static async Task StopAsync()
    {
        Logger.Info(() => "Stopping");

        _rootScope.Dispose();
        await _rootScope.DisposeAsync();

        Logger.Info(() => "Completed");
    }
}