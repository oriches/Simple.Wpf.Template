namespace WpfTemplate.Tests
{
    using System;
    using System.Collections.Generic;
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
        public void cpu_utilisation_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<int>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.CpuUtilisation
                .Subscribe(values.Add);

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
        public void memory_pumps_when_idling()
        {
            // ARRANGE
            var values = new List<Memory>();

            var service = new DiagnosticsService(_idleService.Object, _schedulerService);
            service.Memory
                .Subscribe(values.Add);

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
