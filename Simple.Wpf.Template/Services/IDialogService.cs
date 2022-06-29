using System;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Services;

public interface IDialogService
{
    IObservable<DialogContent> Show { get; }

    void Post(string title, IClosableViewModel viewModel);
}