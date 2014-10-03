namespace Simple.Wpf.Template.Tests
{
    using System.Collections.Generic;
    using NLog;
    using NLog.Targets;

    [Target("TestLog")]
    public sealed class TestLogTarget : TargetWithLayout
    {
        private static readonly List<string> _output;

        public static IEnumerable<string> Output { get { return _output; } }

        public static void Reset()
        {
            _output.Clear();
        }

        static TestLogTarget()
        {
            _output = new List<string>();
        }
        
        protected override void Write(LogEventInfo logEvent)
        {
            var logMessage = this.Layout.Render(logEvent);
            _output.Add(logMessage);
        }
    } 
}