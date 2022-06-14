namespace Simple.Wpf.Template.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Reactive;
    using NUnit.Framework;
    using Services;
    using Template;

    [TestFixture]
    public class HeartbeatFixtures
    {
        [Test]
        public void beats_regularly()
        {
            // ARRANGE
            var hearbeat = new Heartbeat(TimeSpan.FromMilliseconds(200));

            // ACT
            var beats = new List<Unit>();
            hearbeat.Listen.Subscribe(beats.Add);
            
            System.Threading.Thread.Sleep(450);

            // ASSERT
            Assert.That(beats, Is.Not.Empty);
            Assert.That(beats.Count, Is.EqualTo(2));
        }

        [Test]
        public void disposing_stops_the_heart_beating()
        {
            // ARRANGE
            var hearbeat = new Heartbeat(TimeSpan.FromMilliseconds(200));

            // ACT
            var beats = new List<Unit>();
            hearbeat.Listen.Subscribe(beats.Add);

            hearbeat.Dispose();

            System.Threading.Thread.Sleep(450);

            // ASSERT
            Assert.That(beats, Is.Empty);
        }
    }
}
