namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Reactive.Subjects;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class DiagnosticsViewModelFixtures
    {
        private TestScheduler _testScheduler;
        private MockSchedulerService _schedulerService;
        private Mock<IDiagnosticsService> _diagnosticService;
        private Subject<int> _cpuUtilisationSubject;
        private Subject<Memory> _memorySubject;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _diagnosticService = new Mock<IDiagnosticsService>(MockBehavior.Strict);

            _cpuUtilisationSubject = new Subject<int>();
            _diagnosticService.Setup(x => x.CpuUtilisation).Returns(_cpuUtilisationSubject);

            _memorySubject = new Subject<Memory>();
            _diagnosticService.Setup(x => x.Memory).Returns(_memorySubject);
        }

        [Test]
        public void when_created_cpu_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.DefaultCpuString));
        }

        [Test]
        public void when_created_total_memory_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.DefaultTotalMemoryString));
        }

        [Test]
        public void when_created_managed_memory_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.DefaultManagedMemoryString));
        }

        [Test]
        public void cpu_value_is_formatted_when_diagnostics_service_pumps_cpu_utilisation()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _cpuUtilisationSubject.OnNext(42);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));
            
            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo("CPU: 42%"));
        }

        [Test]
        public void cpu_value_is_default_value_when_diagnostics_service_cpu_utilisation_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _cpuUtilisationSubject.OnError(new Exception("blah!"));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.DefaultCpuString));
        }

        [Test]
        public void total_memory_value_is_formatted_when_diagnostics_service_pumps_memory()
        {
            // ARRANGE
            const decimal managedMemory = 1024*1000*4;
            const decimal totalMemory = 1024*1000*42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo("Total Memory: 42.00 Mb"));
        }

        [Test]
        public void total_memory_value_is_default_value_when_diagnostics_service_memory_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _memorySubject.OnError(new Exception("blah!"));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.DefaultTotalMemoryString));
        }

        [Test]
        public void managed_memory_value_is_formatted_when_diagnostics_service_pumps_memory()
        {
            // ARRANGE
            const decimal managedMemory = 1024 * 1000 * 4;
            const decimal totalMemory = 1024 * 1000 * 42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo("Managed Memory: 4.00 Mb"));
        }

        [Test]
        public void managed_memory_value_is_default_value_when_diagnostics_service_memory_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _memorySubject.OnError(new Exception("blah!"));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.DefaultManagedMemoryString));
        }

        [Test]
        public void disposing_unsubscribes_diagnostics_service_stream()
        {
            // ARRANGE
            const decimal managedMemory = 1024 * 1000 * 4;
            const decimal totalMemory = 1024 * 1000 * 42;

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            viewModel.Dispose();

            _memorySubject.OnNext(new Memory(totalMemory, managedMemory));
            _cpuUtilisationSubject.OnNext(42);

            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.DefaultCpuString));
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.DefaultTotalMemoryString));
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.DefaultManagedMemoryString));
        }
    }
}
