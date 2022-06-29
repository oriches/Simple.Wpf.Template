using NLog;

namespace Simple.Wpf.Template.Services;

public interface IConfigurationService
{
    bool EnableAutoShutdown { get; set; }

    LogLevel LogLevel { get; set; }
}