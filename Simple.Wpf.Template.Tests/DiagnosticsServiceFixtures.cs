namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Helpers;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
    using NLog;
    using NUnit.Framework;
    using Services;

    [TestFixture]
    public sealed class DiagnosticsServiceFixtures
    {
        private TestScheduler _testScheduler;
        private ISchedulerService _schedulerService;
        private Mock<IIdleService> _idleService;
        private Subject<Unit> _idling;

        [SetUp]
        public void Setup()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);
            
            _idleService = new Mock<IIdleService>();
            
            _idling = new Subject<Unit>();
            _idleService.Setup(x => x.Idling).Returns(_idling);
        }
        
        [Test]
        public void cpu_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<int>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Cpu.Subscribe(values.Add);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20));

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(20));

            // ASSERT
            Assert.That(values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(2));
        }

        [Test]
        public void memory_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<Memory>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Memory.Subscribe(values.Add);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(values, Is.Not.Empty);
            Assert.That(values.Count, Is.EqualTo(2));
        }

        [Test]
        public void log_pumps_when_nlog_is_used()
        {
            // ARRANGE
            LogHelper.ReconfigureLoggerToLevel(LogLevel.Error);
            var logger = LogManager.GetCurrentClassLogger();

            var message1 = $"Message 1 - {Guid.NewGuid()}";
            var message2 = $"Message 2 - {Guid.NewGuid()}";

            var logValues = new List<string>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Log.Subscribe(logValues.Add);

            //ACT
            logger.Error(message1);
            logger.Error(message2);

            _testScheduler.AdvanceBy(Constants.DiagnosticsLogInterval + Constants.DiagnosticsLogInterval);

            //ASSERT
            Assert.That(logValues.Count(x => x.Contains(message1)) == 1, Is.True);
            Assert.That(logValues.Count(x => x.Contains(message2)) == 1, Is.True);
        }
        
        [Test]
        public void disposing_stops_streams_pumping()
        {
            // ARRANGE
            var called = false;
            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            
            service.Rps.Subscribe(x => { called = true; });
            service.Cpu.Subscribe(x => { called = true; });
            service.Memory.Subscribe(x => { called = true; });

            // ACT
            service.Dispose();

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(called, Is.False);
        }
    }
}
