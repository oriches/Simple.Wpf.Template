using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using NLog;

namespace Simple.Wpf.Template.Views.Helpers;

public static class ButtonHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    public static readonly DependencyProperty DisableDoubleClickProperty =
        DependencyProperty.RegisterAttached("DisableDoubleClick", typeof(bool), typeof(ButtonHelper),
            new PropertyMetadata(default(bool), OnCallback));

    public static bool GetDisableDoubleClick(UIElement element) => (bool)element.GetValue(DisableDoubleClickProperty);

    public static void SetDisableDoubleClick(UIElement element, bool value) =>
        element.SetValue(DisableDoubleClickProperty, value);

    private static void OnCallback(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        if (d is Button button) button.PreviewMouseDown += HandlePreviewMouseDown;
    }

    private static void HandlePreviewMouseDown(object sender, MouseButtonEventArgs args)
    {
        if (args.ClickCount > 1)
        {
            args.Handled = true;
            Logger.Info(() => "Button double click detected and ignored...");
        }
    }
}