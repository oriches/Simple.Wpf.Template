namespace Simple.Wpf.Template.Commands
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;

    public sealed class ObservableCommand : RelayCommand<object>, IObservableCommand, IDisposable
    {
        private readonly Subject<Unit> _executing = new Subject<Unit>();
        private readonly Subject<Unit> _executed = new Subject<Unit>();

        public ObservableCommand(Action execute)
            : base(o => execute())
        {
        }

        public ObservableCommand(Action execute, Func<bool> canExecute)
            : base(o => execute(), o => canExecute())
        {
        }

        public override void Execute(object parameter)
        {
            _executing.OnNext(Unit.Default);

            base.Execute(parameter);

            _executed.OnNext(Unit.Default);
        }

        public void Dispose()
        {
            _executing.OnCompleted();
            _executing.Dispose();

            _executed.OnCompleted();
            _executed.Dispose();
        }

        public IObservable<Unit> Executing { get { return _executing; } }

        public IObservable<Unit> Executed { get { return _executed; } }
    }

    public sealed class ObservableCommand<T> : RelayCommand<T>, IObservableCommand<T>, IDisposable
    {
        private readonly Subject<T> _executing = new Subject<T>();
        private readonly Subject<T> _executed = new Subject<T>();

        public ObservableCommand(Action<T> execute)
            : base(execute)
        {
        }

        public ObservableCommand(Action<T> execute, Predicate<T> canExecute)
            : base(execute, canExecute)
        {
        }

        public override void Execute(object parameter)
        {
            _executing.OnNext((T)parameter);

            base.Execute(parameter);

            _executed.OnNext((T)parameter);
        }

        public void Dispose()
        {
           _executing.OnCompleted();
            _executing.Dispose();

            _executed.OnCompleted();
            _executed.Dispose();
        }

        public IObservable<T> Executing { get { return _executing; } }

        public IObservable<T> Executed { get { return _executed; } }
    }
}
