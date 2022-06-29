using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.ViewModels;

public abstract class ClosableViewModel : DisposableViewModel, IClosableViewModel
{
    private readonly Subject<CloseResult> _closed;

    protected ClosableViewModel() => _closed = new Subject<CloseResult>().DisposeWith(this);

    public IObservable<CloseResult> Closed => _closed.AsObservable();

    public void Cancel() => Close(CloseResult.Cancel);

    protected virtual void Close(CloseResult closeResult)
    {
        if (!_closed.IsDisposed)
            _closed.OnNext(closeResult);
    }
}