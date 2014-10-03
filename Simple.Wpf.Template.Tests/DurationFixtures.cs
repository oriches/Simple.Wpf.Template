namespace Simple.Wpf.Template.Tests
{
    using System.IO;
    using System.Linq;
    using System.Threading;
    using NLog;
    using NLog.Config;
    using NUnit.Framework;
    using Template;

    [TestFixture]
    public class DurationFixtures
    {
        [Test]
        public void logs_duration_when_debug_level_is_enabled()
        {
            // ARRANGE
            var logger = GetLogger("NLog.Test.Debug.config");
            
            // ACT
            using (Duration.Measure(logger, "Message 1"))
            {
                Thread.Sleep(100);
            }

            // ASSERT
            Assert.That(TestLogTarget.Output, Is.Not.Empty);
            Assert.That(TestLogTarget.Output.First(), Contains.Substring("Message 1"));
        }

        [Test]
        public void does_not_log_duration_when_debug_log_level_is_enabled()
        {
            // ARRANGE
            var logger = GetLogger("NLog.Test.Info.config");

            // ACT
            using (Duration.Measure(logger, "Message 1"))
            {
                Thread.Sleep(100);
            }

            // ASSERT
            Assert.That(TestLogTarget.Output, Is.Empty);
        }
        
        private static Logger GetLogger(string configFile)
        {
            TestLogTarget.Reset();
            ConfigurationItemFactory.Default.Targets.RegisterDefinition("TestLog", typeof (TestLogTarget));
            
            var configPath = Path.Combine(Directory.GetCurrentDirectory(), configFile);
            LogManager.Configuration = new XmlLoggingConfiguration(configPath, true);

            return LogManager.GetCurrentClassLogger();
        }
    }
}