using Autofac;
using NUnit.Framework;
using OneLauncher.Core.Container;

namespace OneLauncher.Tests.Framework
{
    [TestFixture]
    public class ContainerOverriderTests
    {
        [Test]
        public void ShouldOverrideCurrentContainerAndRestoreItAfterTheDispose()
        {
            var initialContainer = OneLauncherContainer.Instance;

            var newContainer = new ContainerBuilder().Build();
            using (ContainerOverrider.Override(newContainer))
            {
                Assert.That(OneLauncherContainer.Instance, Is.SameAs(newContainer));
            }

            // And now it should be back to normal
            Assert.That(OneLauncherContainer.Instance, Is.SameAs(initialContainer));
        }
    }
}
