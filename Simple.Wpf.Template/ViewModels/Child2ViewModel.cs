namespace Simple.Wpf.Template.ViewModels
{
    using System;
    using System.Reactive.Disposables;
    using System.Windows.Input;
    using Commands;
    using NLog;
    using Services;

    public sealed class Child2ViewModel : BaseViewModel, IDisposable
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IDisposable _disposable;

        public Child2ViewModel(IGestureService gestureService)
        {
            DelayCommand = new RelayCommand(() =>
            {
                gestureService.SetBusy();
                System.Threading.Thread.Sleep(3123);
            });

            _disposable = Disposable.Create(() =>
            {
                DelayCommand = null;
            });
        }

        public void Dispose()
        {
            using (Duration.Measure(Logger, "Dispose"))
            {
                _disposable.Dispose();
            }
        }

        public string Title => "Child 2";

        public ICommand DelayCommand { get; private set; }
    }
}
