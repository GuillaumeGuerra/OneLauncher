using System;
using NUnit.Framework;
using OneLauncher.Core.Container;

namespace OneLauncher.Commands.Tests
{
    [SetUpFixture]
    public class AssemblyInitialize
    {
        private string _backup;

        [OneTimeSetUp]
        public void Setup()
        {
            _backup = Environment.CurrentDirectory;

            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            OneLauncherContainer.ConfigureDependencyInjection();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            Environment.CurrentDirectory = _backup;
        }
    }
}
