using System.IO;
using NUnit.Framework;
using OneLauncher.Services.CommandLauncher;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Tests.Framework;

namespace OneLauncher.Tests.CommandLauncher
{
    [TestFixture]
    public class XPathReplacerTests : CommonCommandLauncherTests<XPathReplacer, XPathReplacerCommand>
    {
        [Test]
        public void ShouldReplaceXmlNodeWhenFileAndXPathAreValid()
        {
            using (var directory = new TemporaryDirectory())
            {
                // First, we'll copy the reference file into the temporary directory
                File.Copy("Data/OmniLauncher.Tests.dll.config", $"{directory.Location}/OmniLauncher.Tests.dll.config");

                new XPathReplacer().Execute(new XPathReplacerCommand()
                {
                    FilePath = $"{directory.Location}/OmniLauncher.Tests.dll.config",
                    XPath = @"configuration/appSettings/add[@key=""sizeOfMyAss""]/@value",
                    Value = "No no, not so big"
                });

                // Now we can compare the reference file with the one that was replaced
                var actual = File.ReadAllText($"{directory.Location}/OmniLauncher.Tests.dll.config");
                var expected = File.ReadAllText("Data/OmniLauncher.Tests.dll.expected.config");

                Assert.That(actual, Is.EqualTo(expected));
            }
        }

        [Test]
        public void ShouldThrowWhenFileToEditCanNotBeFound()
        {
            Assert.That(() => new XPathReplacer().Execute(new XPathReplacerCommand() { FilePath = "//john/doe" }), 
                Throws.Exception.InstanceOf<FileNotFoundException>().With.Message.EqualTo("Unable to find file [//john/doe] for xpath replacement"));
        }
    }
}