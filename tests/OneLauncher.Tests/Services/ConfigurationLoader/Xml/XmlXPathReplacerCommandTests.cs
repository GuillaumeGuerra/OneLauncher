using NUnit.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.ConfigurationLoader.Xml;

namespace OneLauncher.Tests.Services.ConfigurationLoader.Xml
{
    [TestFixture]
    public class XmlXPathReplacerCommandTests : CommonXmlCommandTests<XmlXPathReplacerCommand, XPathReplacerCommand>
    {
        protected override string GetTemplateXml()
        {
            return @"<XPath FilePath=""Jar-Jar.Binks"" XPath=""is/he/a/asshole"" Value=""without a doubt yes"" />";
        }

        protected override void AssertXmlCommandDeserializedFromXml(XmlXPathReplacerCommand xmlCommand)
        {
            Assert.That(xmlCommand.FilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(xmlCommand.XPath, Is.EqualTo("is/he/a/asshole"));
            Assert.That(xmlCommand.Value, Is.EqualTo("without a doubt yes"));
        }

        protected override XmlXPathReplacerCommand GetTemplateXmlCommand()
        {
            return new XmlXPathReplacerCommand()
            {
                FilePath = "Jar-Jar.Binks",
                XPath = "is/he/a/asshole",
                Value = "without a doubt yes"
            };
        }

        protected override void AssertCommandExtractedFromXmlCommand(XPathReplacerCommand command)
        {
            Assert.That(command.FilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(command.XPath, Is.EqualTo("is/he/a/asshole"));
            Assert.That(command.Value, Is.EqualTo("without a doubt yes"));
        }
    }
}