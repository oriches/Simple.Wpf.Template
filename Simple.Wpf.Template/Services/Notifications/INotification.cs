using System.Threading;
using System.Threading.Tasks;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.Services.Notifications
{
    public interface INotification
    {
        NotificationType Type { get; }

        Task ExecuteAsync(object[] parameters, CancellationToken cancellationToken);
    }
}
