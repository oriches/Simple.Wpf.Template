using System;
using System.Reactive.Disposables;
using NLog;

namespace Simple.Wpf.Template.Modules;

public abstract class BaseModule : IDisposable
{
    protected static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _disposables.Dispose();
    }

    public static implicit operator CompositeDisposable(BaseModule instance) => instance._disposables;
}