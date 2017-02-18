using System;
using NUnit.Framework;
using OneLauncher.Services.CommandLauncher;
using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.Tests.CommandLauncher
{
    [TestFixture]
    public class BaseCommandLauncherTests
    {
        [Test]
        public void ShouldThrowWhenTypeOfCommandDoesNotMatch()
        {
            var expectedMessage =
                "Invalid command type [OneLauncher.Services.ConfigurationLoader.XPathReplacerCommand] given to CommandLauncher plugin [OneLauncher.Tests.CommandLauncher.BaseCommandLauncherTests+MockCommandLauncher]";

            Assert.That(() => new MockCommandLauncher().Execute(new XPathReplacerCommand()), Throws.Exception.TypeOf<NotSupportedException>().With.Message.EqualTo(expectedMessage));
        }

        private class MockCommandLauncher : BaseCommandLauncher<ExecuteCommand>
        {
            protected override void DoExecute(ExecuteCommand command)
            {
                // Should not be called !
                throw new NotImplementedException();
            }
        }
    }
}