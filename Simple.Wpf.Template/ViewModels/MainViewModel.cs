namespace Simple.Wpf.Template.ViewModels
{
    public sealed class MainViewModel : BaseViewModel
    {
        public MainViewModel(Child1ViewModel child1, Child2ViewModel child2, DiagnosticsViewModel diagnosticsViewModel)
        {
            Diagnostics = diagnosticsViewModel;
            Child1 = child1;
            Child2 = child2;
        }

        public string Title { get { return "Main"; } }

        public Child1ViewModel Child1 { get; private set; }

        public Child2ViewModel Child2 { get; private set; }

        public DiagnosticsViewModel Diagnostics { get; private set; }
    }
}
