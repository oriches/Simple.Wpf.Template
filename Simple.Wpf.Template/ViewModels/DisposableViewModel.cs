using System;
using System.Reactive.Disposables;

namespace Simple.Wpf.Template.ViewModels;

public abstract class DisposableViewModel : BaseViewModel, IDisposableViewModel
{
    private readonly CompositeDisposable _disposables = new();

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _disposables.Dispose();
    }

    public static implicit operator CompositeDisposable(DisposableViewModel instance) => instance._disposables;
}