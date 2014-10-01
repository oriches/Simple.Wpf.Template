namespace WpfTemplate.Tests
{
    using System;
    using System.Reactive;
    using System.Reactive.Subjects;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
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
            
            _idleService = new Mock<IIdleService>(MockBehavior.Strict);
            
            _idling = new Subject<Unit>();
            _idleService.Setup(x => x.Idling).Returns(_idling);
        }

        [Test]
        public void intialised_pumps()
        {
            // ARRANGE
            var initialised = false;

            // ACT
            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Initialised
                .Subscribe(x => { initialised = true; });

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(initialised, Is.True);
        }

        [Test]
        public void cpu_utilisation_pumps()
        {
            // ARRANGE
            int? cpu = null;

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.CpuUtilisation
                .Subscribe(x => { cpu = x; });

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(cpu, Is.Not.Null);
        }

        [Test]
        public void memory_pumps()
        {
            // ARRANGE
            Memory value = null;

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Memory
                .Subscribe(x => { value = x; });

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ACT
            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(value, Is.Not.Null);
        }

        [Test]
        public void disposing_stops_streams_pumping()
        {
            // ARRANGE
            var called = false;
            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.CpuUtilisation
                .Subscribe(x => { called = true; });
            
            service.Memory
                .Subscribe(x => { called = true; });

            // ACT
            service.Dispose();

            _idling.OnNext(Unit.Default);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(10));

            // ASSERT
            Assert.That(called, Is.False);
        }
    }
}
