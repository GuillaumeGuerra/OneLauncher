using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using OneLauncher.Commands.Commands.ExecuteCommand;

namespace OneLauncher.Tests.Services.CommandLauncher
{
    [TestFixture]
    public class ExecuteCommandLauncherTests : CommonCommandLauncherTests<ExecuteCommandLauncher, ExecuteCommand>
    {
        [Test]
        public void ShouldLaunchConsoleAppWhenCommandIsValid()
        {
            // So far, no process should be running
            Assert.That(Process.GetProcessesByName("TestConsoleApplication"), Has.Length.EqualTo(0));

            new ExecuteCommandLauncher().Execute(new ExecuteCommand() { Command = "TestConsoleApplication.exe" });

            // Wait for a few milliseconds, to ensure the process had time to start
            Thread.Sleep(1000);

            var processes = Process.GetProcessesByName("TestConsoleApplication");
            Assert.That(processes, Has.Length.EqualTo(1));

            // Now we try to kill the process, no need to keep it alive
            try
            {
                processes.First().Kill();
            }
            catch (Exception)
            {
                // The process may have commited suicide on its own
            }
        }
    }
}