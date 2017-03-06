using System;
using NUnit.Framework;
using OneLauncher.Core.Container;

namespace OneLauncher.Tests
{
    [SetUpFixture]
    public class AssemblyInitialize
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            OneLauncherContainer.ConfigureDependencyInjection();
        }
    }
}
