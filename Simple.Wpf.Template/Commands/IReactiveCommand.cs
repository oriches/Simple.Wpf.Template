using System;
using System.Reactive;
using System.Windows.Input;

namespace Simple.Wpf.Template.Commands;

public interface IReactiveCommand : IObservable<Unit>, ICommand, IDisposable
{
    void Execute();
}

public interface IReactiveCommand<out T> : IObservable<T>, ICommand, IDisposable
{
}