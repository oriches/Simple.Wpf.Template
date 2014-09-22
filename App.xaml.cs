namespace WpfTemplate
{
    using System.Windows;

    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            BootStrapper.Start();

            var viewModel = BootStrapper.RootVisual;
            var window = new MainWindow { DataContext = viewModel };

            window.Show();
        }
    }
}
