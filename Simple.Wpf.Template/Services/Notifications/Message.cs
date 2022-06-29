using System;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Autofac.Core.Activators.Reflection;
using JetBrains.Annotations;
using Simple.Wpf.Template.Models;
using Windows.UI.Notifications;

namespace Simple.Wpf.Template.Services.Notifications;

[UsedImplicitly]
public sealed class Message : INotification, IRegisteredService, IEquatable<Message>
{
    public bool Equals(Message other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type;
    }

    public override bool Equals(object obj) => ReferenceEquals(this, obj) || obj is Message other && Equals(other);

    public override int GetHashCode() => Type.GetHashCode();

    public static bool operator ==(Message left, Message right) => Equals(left, right);

    public static bool operator !=(Message left, Message right) => !Equals(left, right);

    public Message()
    {
        var directory = Path.GetDirectoryName(typeof(MessageWithSnooze).Assembly.Location);
        if (directory == null)
            throw new Exception("Directory is NULL");

        var templates = Path.Combine(directory, "Services", "Notifications", "Templates");
        Template = File.ReadAllText(Path.Combine(templates, templates, "message.xml"));
    }

    private string Template { get;  }

    public NotificationType Type => NotificationType.Message;

    public Task ExecuteAsync(object[] parameters, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            var data = string.Format(Template, parameters.First(), null, null);

            var xml = new XmlDocument();
            xml.LoadXml(data);

            cancellationToken.ThrowIfCancellationRequested();

            var toastNotifier = ToastNotificationManager.CreateToastNotifier(Constants.Notifications.ApplicationId);
            var toast = new ToastNotification(xml);

            toastNotifier.Show(toast);
        }, cancellationToken);
    }
}