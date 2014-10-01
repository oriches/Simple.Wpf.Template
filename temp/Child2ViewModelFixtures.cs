namespace WpfTemplate.Tests
{
    using System;
    using System.Windows.Input;
    using Extensions;
    using Moq;
    using NUnit.Framework;
    using Services;
    using ViewModels;

    [TestFixture]
    public sealed class Child2ViewModelFixtures
    {
        [Test]
        public void busy_gestures_when_delay_command_execute()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            gestureService.Setup(x => x.SetBusy()).Verifiable();

            var viewModel = new Child2ViewModel(gestureService.Object);

            // ACT
            viewModel.DelayCommand.Execute(null);

            // ASSERT
            gestureService.VerifyAll();
        }

        [Test]
        public void disposing_clears_commands()
        {
            // ARRANGE
            var gestureService = new Mock<IGestureService>(MockBehavior.Strict);
            var viewModel = new Child2ViewModel(gestureService.Object);
            
            // ACT
            viewModel.Dispose();

            // ASSERT
            var commandProperties = TestHelper.PropertiesImplementingInterface<ICommand>(viewModel);
            commandProperties.ForEach(x => Assert.That(x.GetValue(viewModel, null), Is.Null));
        }
    }
}
