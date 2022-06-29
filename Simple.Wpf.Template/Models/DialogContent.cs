using System;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Models;

public sealed class DialogContent
{
    public DialogContent(string title, IClosableViewModel viewModel)
    {
        Title = title;
        ViewModel = viewModel;
    }

    public IClosableViewModel ViewModel { get; }
    
    public string Title { get; }
}