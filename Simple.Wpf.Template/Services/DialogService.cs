using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Models;
using Simple.Wpf.Template.ViewModels;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class DialogService : DisposableService, IDialogService, IRegisteredService
{
    private readonly IDateTimeService _dateTimeService;
    private readonly Subject<DialogContent> _show;
    private readonly ConcurrentQueue<DialogContent> _waitingContent = new();

    public DialogService(IDateTimeService dateTimeService)
    {
        _dateTimeService = dateTimeService;
        _show = new Subject<DialogContent>().DisposeWith(this);

        Disposable.Create(() => _waitingContent.Clear())
            .DisposeWith(this);
    }

    public void Post(string title, IClosableViewModel viewModel)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Error Title is invalid!", nameof(title));

        if (viewModel == null)
            throw new ArgumentNullException(nameof(viewModel));

        var newContent = new DialogContent(title, viewModel);

        newContent.ViewModel.Closed
            .Take(1)
            .Subscribe(x =>
            {
                _waitingContent.TryDequeue(out _);

                if (_waitingContent.TryPeek(out var nextContent))
                    _show.OnNext(nextContent);
            })
            .DisposeWith(this);

        _waitingContent.Enqueue(newContent);
        if (_waitingContent.Count == 1)
            _show.OnNext(newContent);
    }

    public IObservable<DialogContent> Show => _show.AsObservable();
}