using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using NLog;

namespace Simple.Wpf.Template.Helpers;

public static class ApplicationCleanup
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    private static readonly ConcurrentStack<Func<Task>> Tasks = new();

    private static Window _window;
    private static Func<Task> _shutdownFunc;

    public static void Register(Window window)
    {
        if (_window != null)
            throw new ArgumentException("Main Window already Registered!");

        _window = window;
        _window.Closing += HandleClosing;
    }

    public static void RegisterForCleanup(Func<Task> func)
    {
        Tasks.Push(func);
    }

    public static void RegisterForShutdown(Func<Task> func)
    {
        if (_shutdownFunc != null)
            throw new ArgumentException("Shutdown Cleanup already defined!", nameof(func));

        _shutdownFunc = func;
    }

    private static void HandleClosing(object sender, CancelEventArgs args)
    {
        if (args.Cancel)
        {
            Logger.Info(() => $"Shutdown Suspended - Cancelled by another Handler, Cancel=[{args.Cancel}]");
            return;
        }

        Logger.Info(() => "Shutdown Started...");

        _window.Closing -= HandleClosing;

        if (Tasks.Any())
        {
            Logger.Info(() => "Async Cleanup Required");

            _window.Visibility = Visibility.Collapsed;
            args.Cancel = true;

            ExecuteAsyncCleanup();
        }
    }

    private static void ExecuteAsyncCleanup()
    {
        Logger.Info(() => $"Async Cleanup Started, Count=[{Tasks.Count}]");
        _window.Dispatcher.Invoke(async () => await ExecuteAsyncTask());
    }

    private static async Task ExecuteAsyncTask()
    {
        if (Tasks.TryPop(out var task))
        {
            await task();
            await _window.Dispatcher.Invoke(async () => await ExecuteAsyncTask());
        }
        else
        {
            Logger.Info(() => "Async Cleanup Completed");
            if (_shutdownFunc != null)
            {
                Logger.Info(() => "Async Shutdown Started");
                await _window.Dispatcher.InvokeAsync(async () => await _shutdownFunc());
            }

            _window.Close();
        }
    }
}