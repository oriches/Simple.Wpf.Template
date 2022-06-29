// ReSharper disable ConvertClosureToMethodGroup

using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Windows.Input;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Services;

// ReSharper disable StaticMemberInGenericType

namespace Simple.Wpf.Template.Commands;

public sealed class ReactiveCommand : ReactiveCommand<object>
{
    private ReactiveCommand(IObservable<bool> canExecute)
        : base(canExecute.StartWith(false))
    {
    }

    public new static ReactiveCommand<object> Create() => ReactiveCommand<object>.Create(Observable.Return(true));

    public new static ReactiveCommand<object> Create(IObservable<bool> canExecute) =>
        ReactiveCommand<object>.Create(canExecute);
}

public class ReactiveCommand<T> : IReactiveCommand<T>
{
    private readonly IDisposable _canDisposable;
    private readonly List<EventHandler> _eventHandlers;
    private readonly Subject<T> _execute;

    private bool _currentCanExecute;

    protected ReactiveCommand(IObservable<bool> canExecute)
    {
        _eventHandlers = new List<EventHandler>(8);

        _canDisposable = canExecute.Subscribe(x =>
        {
            _currentCanExecute = x;
            CommandManager.InvalidateRequerySuggested();
        });

        _execute = new Subject<T>();
    }

    public virtual void Execute(object parameter)
    {
        var typedParameter = parameter is T o ? o : default;

        if (CanExecute(typedParameter)) _execute.OnNext(typedParameter);
    }

    public virtual bool CanExecute(object parameter) => _currentCanExecute;

    public event EventHandler CanExecuteChanged
    {
        add
        {
            _eventHandlers.Add(value);
            CommandManager.RequerySuggested += value;
        }
        remove
        {
            _eventHandlers.Remove(value);
            CommandManager.RequerySuggested -= value;
        }
    }

    public void Dispose()
    {
        GC.SuppressFinalize(this);

        using (Duration.Measure(() => $"Dispose=[{GetType().Name}]"))
        {
            _eventHandlers.ForEach(x => CommandManager.RequerySuggested -= x);
            _eventHandlers.Clear();

            _canDisposable.Dispose();

            _execute.OnCompleted();
            _execute.Dispose();
        }
    }

    public IDisposable Subscribe(IObserver<T> observer) =>
        _execute.ActivateGestures()
            .Subscribe(observer);

    public static ReactiveCommand<T> Create() => new(Observable.Return(true));

    public static ReactiveCommand<T> Create(IObservable<bool> canExecute) => new(canExecute);
}