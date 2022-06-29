using System;

namespace Simple.Wpf.Template.Services;

public interface IDateTimeService
{
    DateTime Date { get; }

    DateTime DateTime { get; }

    TimeSpan Time { get; }

    TimeSpan LocalTime { get; }

    DateTime LocalDateTime { get; }
}