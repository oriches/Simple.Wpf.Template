using System.Collections.Generic;
using NLog;

namespace Simple.Wpf.Template.ViewModels;

public interface ISettingsViewModel : IDisposableViewModel
{
    IEnumerable<LogLevel> LogLevels { get; }

    LogLevel LogLevel { get; set; }

    bool EnableAutoShutdown { get; set; }
}