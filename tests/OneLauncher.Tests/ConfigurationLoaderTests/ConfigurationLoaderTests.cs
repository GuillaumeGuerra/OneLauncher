using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Tests.Framework;

namespace OneLauncher.Tests.ConfigurationLoaderTests
{
    [TestFixture]
    public class ConfigurationLoaderTests
    {
        [Test]
        public void ShouldReadAllFilesLocatedInTheConfigurationDirectoryWithTheRightPlugin()
        {
            using (var directory = new TemporaryDirectory())
            {
                File.WriteAllText($"{directory.Location}\\file1.xml", "");
                File.WriteAllText($"{directory.Location}\\file2.json", "");

                // This one is not known by any plugin, it should be ignored
                File.WriteAllText($"{directory.Location}\\file3.bin", "");

                var firstNode = new LaunchersNode();
                var secondNode = new LaunchersNode();

                var xmlLoader = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
                xmlLoader.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                    .Returns<string>(s => s.EndsWith(".xml"))
                    .Verifiable();
                xmlLoader.Setup(mock => mock.Load($"{directory.Location}\\file1.xml")).Returns(firstNode).Verifiable();

                var jsonLoader = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
                jsonLoader.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                    .Returns<string>(s => s.EndsWith(".json"))
                    .Verifiable();
                jsonLoader.Setup(mock => mock.Load($"{directory.Location}\\file2.json")).Returns(secondNode).Verifiable();

                var loader = new ConfigurationLoader() { AllConfigurationProcessors = new[] { xmlLoader.Object, jsonLoader.Object } };
                var launchers = loader.LoadConfiguration(directory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(2));
                Assert.That(launchers[0], Is.SameAs(firstNode));
                Assert.That(launchers[1], Is.SameAs(secondNode));

                xmlLoader.VerifyAll();
            }
        }
    }
}
