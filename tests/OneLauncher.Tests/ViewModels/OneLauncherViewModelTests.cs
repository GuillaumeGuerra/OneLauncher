using System.Threading;
using NUnit.Framework;
using OneLauncher.ViewModels;

namespace OneLauncher.Tests.ViewModels
{
    [TestFixture]
    public class OneLauncherViewModelTests
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        [Ignore("Work in progress")]
        public void ShouldInitializeLaunchersWhenLoaded()
        {
            var vm = new OneLauncherViewModel();

            // As long as the load is not completed, the VM should maintain the view closed
            Assert.That(vm.IsOpened, Is.False);

            Assert.That(vm.Launchers, Has.Count.EqualTo(0));

            vm.LoadedCommand.Execute(null);

            Thread.Sleep(5000);

            Assert.That(vm.Launchers, Has.Count.GreaterThan(0));

            // Now it should be opened
            Assert.That(vm.IsOpened, Is.True);
        }
    }
}
