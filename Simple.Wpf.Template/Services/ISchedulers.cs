using System.Reactive.Concurrency;

namespace Simple.Wpf.Template.Services;

public interface ISchedulers
{
    IScheduler Dispatcher { get; }

    IScheduler TaskPool { get; }

    IScheduler NamedThread(string name);
}