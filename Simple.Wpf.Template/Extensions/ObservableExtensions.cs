using System;
using System.Reactive.Linq;
using JetBrains.Annotations;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Extensions;

[UsedImplicitly]
public static class ObservableExtensions
{
    public static IGestureService GestureService { get; private set; }

    public static void Initialize(IGestureService gestureService)
    {
        GestureService = gestureService;
    }

    public static IObservable<T> ActivateGestures<T>(this IObservable<T> observable)
    {
        if (GestureService == null)
            throw new ArgumentNullException(nameof(GestureService), "Gesture Service has not been defined!");

        return observable.Do(_ => GestureService.SetBusy());
    }
}