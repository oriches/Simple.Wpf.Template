using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Simple.Wpf.Template.Commands;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.ViewModels;

[UsedImplicitly]
public sealed class ExceptionViewModel : ClosableViewModel, IExceptionViewModel, IRegisteredViewModel
{
    private readonly IApplicationService _applicationService;
    private readonly Exception _exception;
    
    public ExceptionViewModel(Exception exception, IApplicationService applicationService)
    {
        _exception = exception;
        _applicationService = applicationService;

        OpenLogFolderCommand = ReactiveCommand.Create(Observable.Return(_applicationService.LogFolder != null))
            .DisposeWith(this);

        CopyCommand = ReactiveCommand.Create(Observable.Return(exception != null))
            .DisposeWith(this);

        ContinueCommand = ReactiveCommand.Create()
            .DisposeWith(this);

        ExitCommand = ReactiveCommand.Create()
            .DisposeWith(this);

        RestartCommand = ReactiveCommand.Create()
            .DisposeWith(this);

        OpenLogFolderCommand.ActivateGestures()
            .Subscribe(x => OpenLogFolder())
            .DisposeWith(this);

        CopyCommand.ActivateGestures()
            .Subscribe(x => Copy())
            .DisposeWith(this);

        ContinueCommand.ActivateGestures()
            .Subscribe(x => Continue())
            .DisposeWith(this);

        ExitCommand
            .ActivateGestures()
            .Subscribe(x => Exit())
            .DisposeWith(this);

        RestartCommand
            .ActivateGestures()
            .Subscribe(x => Restart())
            .DisposeWith(this);

        Closed.Take(1)
            .Subscribe(x =>
            {
                // Force all other potential exceptions to be realized
                // from the Finalizer thread to surface to the UI
                GC.Collect(2, GCCollectionMode.Forced);
                GC.WaitForPendingFinalizers();
            })
            .DisposeWith(this);
    }

    public IReactiveCommand<object> CopyCommand { get; }

    public IReactiveCommand<object> OpenLogFolderCommand { get; }

    public IReactiveCommand<object> ContinueCommand { get; }

    public IReactiveCommand<object> ExitCommand { get; }

    public IReactiveCommand<object> RestartCommand { get; }

    public string Message => _exception?.Message;

    private void Copy() => _applicationService.CopyToClipboard(_exception.ToString());

    private void OpenLogFolder() => _applicationService.OpenFolder(_applicationService.LogFolder);

    private void Exit() => _applicationService.Exit();

    private void Restart() => _applicationService.Restart();

    private void Continue() => Close(CloseResult.Okay);
}