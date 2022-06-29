using System.Reactive.Concurrency;
using JetBrains.Annotations;
using Microsoft.Reactive.Testing;
using Simple.Wpf.Template.Services;

namespace Simple.Wpf.Template.Tests;

[UsedImplicitly]
public sealed class MockSchedulers : ISchedulers
{
    private readonly TestScheduler _testScheduler;

    public MockSchedulers(TestScheduler testScheduler) => _testScheduler = testScheduler;

    public IScheduler Dispatcher => _testScheduler;

    public IScheduler TaskPool => _testScheduler;

    public IScheduler NamedThread(string name) => _testScheduler;
}