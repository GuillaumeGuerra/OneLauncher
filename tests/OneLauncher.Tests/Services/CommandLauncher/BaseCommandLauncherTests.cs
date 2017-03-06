using System;
using NUnit.Framework;
using OneLauncher.Commands.Commands.ExecuteCommand;
using OneLauncher.Commands.Commands.XPathReplacer;
using OneLauncher.Core.Commands;

namespace OneLauncher.Tests.Services.CommandLauncher
{
    [TestFixture]
    public class BaseCommandLauncherTests
    {
        [Test]
        public void ShouldThrowWhenTypeOfCommandDoesNotMatch()
        {
            var expectedMessage =
                "Invalid command type [OneLauncher.Commands.Commands.XPathReplacer.XPathReplacerCommand] given to CommandLauncher plugin [OneLauncher.Tests.Services.CommandLauncher.BaseCommandLauncherTests+MockCommandLauncher]";

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