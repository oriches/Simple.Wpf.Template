namespace Simple.Wpf.Template.Tests
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Extensions;
    using NLog;

    public static class TestHelper
    {
        private static readonly IEnumerable<LogLevel> AllLevels = new[]
            {
                LogLevel.Trace,
                LogLevel.Debug,
                LogLevel.Info,
                LogLevel.Warn,
                LogLevel.Error,
                LogLevel.Fatal,
            };

        public static IEnumerable<PropertyInfo> PropertiesImplementingInterface<T>(object instance)
        {
            return instance.GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(T) || x.PropertyType.GetInterfaces().Any(y => y == typeof(T)));
        }

        public static void ReconfigureLoggerToLevel(LogLevel level)
        {
            var disableLevels = AllLevels.Where(x => x < level)
                .ToArray();

            var enableLevels = AllLevels.Where(x => x >= level)
                .ToArray();

            foreach (var rule in LogManager.Configuration.LoggingRules)
            {
                var localRule = rule;

                disableLevels.ForEach(localRule.DisableLoggingForLevel);
                enableLevels.ForEach(localRule.EnableLoggingForLevel);
            }
            
            LogManager.ReconfigExistingLoggers();
        }
    }
}