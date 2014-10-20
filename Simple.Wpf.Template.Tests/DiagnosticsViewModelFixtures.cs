namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Linq;
    using System.Reactive.Subjects;
    using Helpers;
    using Microsoft.Reactive.Testing;
    using Models;
    using Moq;
    using NLog;
    using NUnit.Framework;
    using Services;
    using Template;
    using ViewModels;

    [TestFixture]
    public sealed class DiagnosticsViewModelFixtures
    {
        private TestScheduler _testScheduler;
        private MockSchedulerService _schedulerService;
        private Mock<IDiagnosticsService> _diagnosticService;
        private Subject<int> _cpuSubject;
        private Subject<Memory> _memorySubject;
        private Subject<int> _fpsSubject;

        [SetUp]
        public void SetUp()
        {
            _testScheduler = new TestScheduler();
            _schedulerService = new MockSchedulerService(_testScheduler);

            _diagnosticService = new Mock<IDiagnosticsService>();

            _fpsSubject = new Subject<int>();
            _diagnosticService.Setup(x => x.Fps).Returns(_fpsSubject);

            _cpuSubject = new Subject<int>();
            _diagnosticService.Setup(x => x.Cpu).Returns(_cpuSubject);

            _memorySubject = new Subject<Memory>();
            _diagnosticService.Setup(x => x.Memory).Returns(_memorySubject);
        }

        [Test]
        public void exposes_log_messages()
        {
            // ARRANGE
            LogHelper.ReconfigureLoggerToLevel(LogLevel.Error);
            var logger = LogManager.GetCurrentClassLogger();
            
            var message1 = string.Format("Message 1 - {0}", Guid.NewGuid());
            var message2 = string.Format("Message 2 - {0}", Guid.NewGuid());

            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            logger.Error(message1);
            logger.Error(message2);

            _testScheduler.AdvanceBy(Constants.DiagnosticsLogInterval + Constants.DiagnosticsLogInterval);

            //ACT
            var log = viewModel.Log.ToArray();

            //ASSERT
            Assert.That(log.Count(x => x.Contains(message1)) == 1, Is.True);
            Assert.That(log.Count(x => x.Contains(message2)) == 1, Is.True);
        }

        [Test]
        public void when_created_fps_is_default_value()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            // ASSERT
            Assert.That(viewModel.Fps, Is.EqualTo(Constants.DefaultFpsString));
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
        public void fps_value_is_formatted_when_diagnostics_service_pumps_fps()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _fpsSubject.OnNext(66);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(2));

            // ASSERT
            Assert.That(viewModel.Fps, Is.EqualTo("Render: 66 FPS"));
        }

        [Test]
        public void fps_value_is_default_value_when_diagnostics_service_fps_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _fpsSubject.OnError(new Exception("blah!"));

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));

            // ASSERT
            Assert.That(viewModel.Fps, Is.EqualTo(Constants.DefaultFpsString));
        }

        [Test]
        public void cpu_value_is_formatted_when_diagnostics_service_pumps_cpu()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _cpuSubject.OnNext(42);

            _testScheduler.AdvanceBy(TimeSpan.FromSeconds(1));
            
            // ASSERT
            Assert.That(viewModel.Cpu, Is.EqualTo("CPU: 42 %"));
        }

        [Test]
        public void cpu_value_is_default_value_when_diagnostics_service_cpu_errors()
        {
            // ARRANGE
            var viewModel = new DiagnosticsViewModel(_diagnosticService.Object, _schedulerService);

            // ACT
            _cpuSubject.OnError(new Exception("blah!"));

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
            _cpuSubject.OnNext(42);
            _fpsSubject.OnNext(65);

            // ASSERT
            Assert.That(viewModel.Fps, Is.EqualTo(Constants.DefaultFpsString));
            Assert.That(viewModel.Cpu, Is.EqualTo(Constants.DefaultCpuString));
            Assert.That(viewModel.TotalMemory, Is.EqualTo(Constants.DefaultTotalMemoryString));
            Assert.That(viewModel.ManagedMemory, Is.EqualTo(Constants.DefaultManagedMemoryString));
        }
    }
}
