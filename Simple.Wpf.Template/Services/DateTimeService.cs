using System;
using JetBrains.Annotations;

namespace Simple.Wpf.Template.Services;

[UsedImplicitly]
public sealed class DateTimeService : IDateTimeService, IRegisteredService
{
    public DateTime Date => DateTimeOffset.UtcNow.UtcDateTime.Date;

    public DateTime DateTime => DateTimeOffset.UtcNow.UtcDateTime;

    public TimeSpan Time => DateTimeOffset.UtcNow.UtcDateTime.TimeOfDay;

    public TimeSpan LocalTime => DateTimeOffset.UtcNow.LocalDateTime.TimeOfDay;

    public DateTime LocalDateTime => DateTimeOffset.UtcNow.LocalDateTime;
}