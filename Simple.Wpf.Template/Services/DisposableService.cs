using System;
using System.Reactive.Disposables;

namespace Simple.Wpf.Template.Services;

public abstract class DisposableService : IDisposable
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _disposables.Dispose();
    }

    public static implicit operator CompositeDisposable(DisposableService instance) => instance._disposables;
}