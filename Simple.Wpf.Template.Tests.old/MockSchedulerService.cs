﻿namespace Simple.Wpf.Template.Tests
{
    using System.Reactive.Concurrency;
    using Microsoft.Reactive.Testing;
    using Services;

    public sealed class MockSchedulerService : ISchedulerService
    {
        private readonly TestScheduler _testScheduler;

        public MockSchedulerService(TestScheduler testScheduler)
        {
            _testScheduler = testScheduler;
        }

        public IScheduler Dispatcher => _testScheduler;

        public IScheduler Current => _testScheduler;

        public IScheduler TaskPool => _testScheduler;

        public IScheduler EventLoop => _testScheduler;

        public IScheduler NewThread => _testScheduler;

        public IScheduler StaThread => _testScheduler;
    }
}