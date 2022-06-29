using System;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows;
using NLog;

namespace Simple.Wpf.Template.Modules;

public static class ModuleLoader
{
    public const string NoLaunchModule =
        "Launch Module is not defined, expected to find a single IModule instance with a Context of ModuleContext.Launch";

    public const string MultipleLaunchModule =
        "Mulitple Launch Modules found, this is not allowed, expected to find a single IModule instance with a Context of ModuleContext.Launch";

    private static readonly Logger Logger;

    static ModuleLoader()
    {
        Logger = LogManager.GetCurrentClassLogger();
        Logger.Info(() => "Hello!");
        Logger.Info(() => $"Framework Version, Value=[{Environment.Version}]");
        Logger.Info(() => $"Windows Version, Value=[{Environment.OSVersion}]");
    }

    public static void Start()
    {
        Logger.Info(() => "Loading...");

        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .ToArray();

        var orderedTypeInfos = assemblies.SelectMany(assembly => assembly.GetTypes()
                .Where(type => !type.IsAbstract &&
                               type.IsClass &&
                               type.IsPublic &&
                               type.GetCustomAttribute<ModuleConfigurationAttribute>() != null))
            .Select(type =>
            {
                var attribute = type.GetCustomAttribute<ModuleConfigurationAttribute>();
                return new TypeInfo(type, attribute);
            })
            .GroupBy(typeInfos => typeInfos.Context)
            .OrderBy(group => group.Key)
            .SelectMany(group => group.OrderBy(typeInfo => typeInfo.Position)
                .Select(typeInfo =>
                {
                    Logger.Info(() =>
                        $"Module, Context=[{typeInfo.Context}], Position=[{typeInfo.Position}], Context=[{typeInfo.Type.Name}]");
                    return typeInfo;
                }))
            .ToArray();

        var launchCount = orderedTypeInfos.Count(type => type.Context == ModuleContext.Launch);
        if (launchCount == 0)
        {
            Logger.Error(() => NoLaunchModule);
            throw new Exception(NoLaunchModule);
        }

        if (launchCount > 1)
        {
            Logger.Error(() => MultipleLaunchModule);
            throw new Exception(MultipleLaunchModule);
        }

        var modules = orderedTypeInfos
            .Select(typeAndAttribute => Create(typeAndAttribute.Type))
            .ToArray();

        var disposable = new CompositeDisposable(modules.Reverse());

        Application.Current.Exit += (_, _) =>
        {
            disposable.Dispose();

            Logger.Info(() => "Bye Bye!");
            LogManager.Flush();
            LogManager.Shutdown();
        };

        Logger.Info(() => "Loaded");
    }

    private static BaseModule Create(IReflect type) => (BaseModule)Activator.CreateInstance(type.UnderlyingSystemType);

    [DebuggerDisplay("Context={Context}, Position={Position}, Type={Type.Name}")]
    private sealed class TypeInfo
    {
        public TypeInfo(Type type, ModuleConfigurationAttribute attribute)
        {
            Type = type;
            Context = attribute.Context;
            Position = attribute.Position;
        }

        public Type Type { get; }

        public int Position { get; }

        public ModuleContext Context { get; }
    }
}