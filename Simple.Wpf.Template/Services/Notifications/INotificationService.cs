using System.Threading;
using System.Threading.Tasks;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.Services.Notifications;

public interface INotificationService
{
    Task ExecuteAsync(NotificationType type, object[] parameters, CancellationToken cancellationToken);
}