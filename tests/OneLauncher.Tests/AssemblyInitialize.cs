using System;
using NUnit.Framework;

namespace OneLauncher.Tests
{
    [SetUpFixture]
    public class AssemblyInitialize
    {
        [OneTimeSetUp]
        public void Setup()
        {
            Environment.CurrentDirectory = TestContext.CurrentContext.TestDirectory;
            App.ConfigureDependencyInjection();
        }
    }
}
