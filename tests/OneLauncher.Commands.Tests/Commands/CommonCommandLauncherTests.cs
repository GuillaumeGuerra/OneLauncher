using NUnit.Framework;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Tests.Commands
{
    public class CommonCommandLauncherTests<TCommandLauncher, TCommand>
        where TCommandLauncher : ICommandLauncher, new()
        where TCommand : LauncherCommand, new()
    {
        [Test]
        public void ShouldAcceptOnlyDedicatedLauncherCommand()
        {
            var launcher = new TCommandLauncher();
            Assert.That(launcher.CanProcess(new TCommand()), Is.True);
            Assert.That(launcher.CanProcess(new WrongCommand()), Is.False);
        }

        private class WrongCommand : LauncherCommand
        {
        }
    }
}