using MahApps.Metro.Controls.Dialogs;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Views;

public sealed class MessageDialog : BaseMetroDialog
{
    private readonly DialogContent _content;

    public MessageDialog(DialogContent content)
    {
        _content = content;
        Title = _content.Title;
        Content = _content.ViewModel;
    }

    public IClosableViewModel CloseableContent => _content.ViewModel;
}