namespace WpfTemplate.Commands
{
    using System;
    using System.Reactive;
    using System.Windows.Input;

    public interface IObservableCommand : ICommand
    {
        IObservable<Unit> Executing { get; }

        IObservable<Unit> Executed { get; }
    }

    public interface IObservableCommand<T> : ICommand
    {
        IObservable<T> Executing { get; }

        IObservable<T> Executed { get; }
    }
}
