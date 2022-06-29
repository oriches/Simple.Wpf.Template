using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Simple.Wpf.Template.Models;

namespace Simple.Wpf.Template.Services.Notifications;

public sealed class MessageWithSnooze : INotification, IRegisteredService, IEquatable<MessageWithSnooze>
{
    public MessageWithSnooze()
    {
        var directory = Path.GetDirectoryName(typeof(MessageWithSnooze).Assembly.Location);
        if (directory == null)
            throw new Exception("Directory is NULL");

        var templates = Path.Combine(directory, "Services", "Notifications", "Templates");
        Template = File.ReadAllText(Path.Combine(templates, templates, "messageWithSnooze.xml"));
    }

    private string Template { get; }

    public NotificationType Type => NotificationType.MessageWithSnooze;

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

    public bool Equals(MessageWithSnooze other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Type == other.Type;
    }

    public override bool Equals(object obj) =>
        ReferenceEquals(this, obj) || (obj is MessageWithSnooze other && Equals(other));

    public override int GetHashCode() => Type.GetHashCode();

    public static bool operator ==(MessageWithSnooze left, MessageWithSnooze right) => Equals(left, right);

    public static bool operator !=(MessageWithSnooze left, MessageWithSnooze right) => !Equals(left, right);
}