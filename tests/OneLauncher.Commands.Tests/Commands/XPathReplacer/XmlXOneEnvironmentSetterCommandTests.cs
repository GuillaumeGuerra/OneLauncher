using NUnit.Framework;
using OneLauncher.Commands.Commands.XPathReplacer;

namespace OneLauncher.Commands.Tests.Commands.XPathReplacer
{
    [TestFixture]
    public class XmlXOneEnvironmentSetterCommandTests : CommonXmlCommandTests<XmlXOneEnvironmentSetterCommand, XPathReplacerCommand>
    {
        protected override string GetTemplateXml()
        {
            return @"<SetXOneEnvironment FilePath=""Jar-Jar.Binks"" XOneEnvironment=""TRASH"" />";
        }

        protected override void AssertXmlCommandDeserializedFromXml(XmlXOneEnvironmentSetterCommand xmlCommand)
        {
            Assert.That(xmlCommand.FilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(xmlCommand.XOneEnvironment, Is.EqualTo("TRASH"));
        }

        protected override XmlXOneEnvironmentSetterCommand GetTemplateXmlCommand()
        {
            return new XmlXOneEnvironmentSetterCommand()
            {
                FilePath = "Jar-Jar.Binks",
                XOneEnvironment = "TRASH"
            };
        }

        protected override void AssertCommandExtractedFromXmlCommand(XPathReplacerCommand command)
        {
            Assert.That(command.FilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(command.XPath, Is.EqualTo(@"configuration/appSettings/add[@key=""usedEnvironment""]/@value"));
            Assert.That(command.Value, Is.EqualTo("TRASH"));
        }
    }
}