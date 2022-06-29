using System;
using System.Reactive.Concurrency;
using NLog;
using Simple.Wpf.Template.Services;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Helpers;

public static class ExceptionHelper
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

    private static IDialogService _dialogService;
    private static ISchedulers _schedulers;
    private static Func<Exception, IExceptionViewModel> _factory;

    public static void Initialize(IDialogService dialogService, ISchedulers schedulers,
        Func<Exception, IExceptionViewModel> factory)
    {
        _dialogService = dialogService;
        _schedulers = schedulers;
        _factory = factory;
    }

    public static void Handle(Exception exception)
    {
        if (_dialogService == null)
            throw new ArgumentNullException(nameof(_dialogService), "ExceptionHandler has not been initialized!");

        Logger.Error(exception.ToString);

        _schedulers.Dispatcher.Schedule(() =>
        {
            var viewModel = _factory(exception);
            viewModel.Closed.Subscribe(_ =>
            {
                try
                {
                    _schedulers.Dispatcher.Schedule(() => viewModel.Dispose());
                }
                catch (Exception disposingException)
                {
                    Logger.Error(() => "Failed to dispose of ExceptionViewModel");
                    Logger.Error(disposingException.ToString);
                    throw;
                }
            });

            _dialogService.Post("Error!", viewModel);
        });
    }
}