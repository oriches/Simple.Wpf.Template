namespace WpfTemplate.Tests
{
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
    }
}
