using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Markup;
using Simple.Wpf.Template.Modules;

namespace Simple.Wpf.Template;

[ExcludeFromCodeCoverage]
public partial class App
{
    public App()
    {
        CultureInfo.DefaultThreadCurrentCulture = CultureInfo.CurrentCulture;
        CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.CurrentCulture;

        Thread.CurrentThread.CurrentCulture = CultureInfo.CurrentCulture;
        Thread.CurrentThread.CurrentUICulture = CultureInfo.CurrentCulture;

        var language = XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag);
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(FrameworkElement),
            new FrameworkPropertyMetadata(language));
        FrameworkElement.LanguageProperty.OverrideMetadata(typeof(TextElement),
            new FrameworkPropertyMetadata(language));
    }

    protected override void OnStartup(StartupEventArgs args)
    {
        ModuleLoader.Start();

        base.OnStartup(args);
    }
}