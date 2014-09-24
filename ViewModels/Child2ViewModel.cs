namespace WpfTemplate.ViewModels
{
    using System.Windows.Input;
    using Commands;

    public sealed class Child2ViewModel : BaseViewModel
    {
        public Child2ViewModel()
        {
            DelayCommand = new RelayCommand(Delay);
        }

        public string Title { get { return "Child 2"; } }

        public ICommand DelayCommand { get; private set; }

        private static void Delay()
        {
            System.Threading.Thread.Sleep(3123);
        }
    }
}
