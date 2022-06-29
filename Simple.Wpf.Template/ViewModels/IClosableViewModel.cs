using System;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.ViewModels;

public interface IClosableViewModel : IDisposableViewModel
{
    IObservable<CloseResult> Closed { get; }

    void Cancel();
}