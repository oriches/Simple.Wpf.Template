using System;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class GestureService : DisposableService, IGestureService, IRegisteredService
{
    private readonly DispatcherTimer _timer;
    private bool _isBusy;

    public GestureService()
    {
        _timer = new DispatcherTimer(TimeSpan.Zero, DispatcherPriority.ApplicationIdle, TimerCallback,
            Application.Current.Dispatcher);

        Disposable.Create(() => _timer.Stop())
            .DisposeWith(this);
    }

    public void SetBusy() => SetBusyState(true);

    private void SetBusyState(bool busy)
    {
        if (busy != _isBusy)
        {
            _isBusy = busy;
            Mouse.OverrideCursor = busy ? Cursors.Wait : null;

            if (_isBusy)
                _timer.Start();
        }
    }

    private void TimerCallback(object sender, EventArgs args)
    {
        SetBusyState(false);
        _timer.Stop();
    }
}