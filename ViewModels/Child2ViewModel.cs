namespace WpfTemplate.ViewModels
{
    using System.Windows.Input;
    using Commands;
    using Services;

    public sealed class Child2ViewModel : BaseViewModel
    {
        public Child2ViewModel(IGestureService gestureService)
        {
            DelayCommand = new RelayCommand(() =>
            {
                gestureService.SetBusy();
                System.Threading.Thread.Sleep(3123);
            });
        }

        public string Title { get { return "Child 2"; } }

        public ICommand DelayCommand { get; private set; }
    }
}
