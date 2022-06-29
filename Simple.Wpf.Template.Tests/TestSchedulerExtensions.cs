using Microsoft.Reactive.Testing;

namespace Simple.Wpf.Template.Tests;

public static class TestSchedulerExtensions
{
    public static void AdvanceBy(this TestScheduler testScheduler, TimeSpan timeSpan)
    {
        testScheduler.AdvanceBy(timeSpan.Ticks);
    }
}