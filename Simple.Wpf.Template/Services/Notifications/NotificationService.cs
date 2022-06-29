using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading;
using System.Threading.Tasks;
using Windows.UI.Notifications;
using JetBrains.Annotations;
using Simple.Wpf.Template.Extensions;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.Services.Notifications;

[UsedImplicitly]
public sealed class NotificationService : DisposableService, INotificationService, IRegisteredService
{
    private readonly IEnumerable<INotification> _notifications;

    public NotificationService(IEnumerable<INotification> notifications)
    {
        _notifications = notifications.Distinct().ToArray();

        Disposable.Create(() => ToastNotificationManager.History.Clear(Constants.Notifications.ApplicationId))
            .DisposeWith(this);
    }

    public Task ExecuteAsync(NotificationType type, object[] parameters, CancellationToken cancellationToken) =>
        _notifications.Single(notification => notification.Type == type)
            .ExecuteAsync(parameters, cancellationToken);
}