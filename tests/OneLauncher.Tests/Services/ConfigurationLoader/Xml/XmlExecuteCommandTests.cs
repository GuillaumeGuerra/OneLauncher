using NUnit.Framework;
using OneLauncher.Commands.Commands.ExecuteCommand;
using OneLauncher.Services.ConfigurationLoader.Xml;

namespace OneLauncher.Tests.Services.ConfigurationLoader.Xml
{
    [TestFixture]
    public class XmlExecuteCommandTests : CommonXmlCommandTests<XmlExecuteCommand, ExecuteCommand>
    {
        protected override string GetTemplateXml()
        {
            return @"<Execute Command=""Kill Jar-Jar.cmd"" />";
        }

        protected override void AssertXmlCommandDeserializedFromXml(XmlExecuteCommand xmlCommand)
        {
            Assert.That(xmlCommand.Command, Is.EqualTo("Kill Jar-Jar.cmd"));
        }

        protected override XmlExecuteCommand GetTemplateXmlCommand()
        {
            var xmlCommand = new XmlExecuteCommand()
            {
                Command = "Kill Jar-Jar.cmd"
            };
            return xmlCommand;
        }

        protected override void AssertCommandExtractedFromXmlCommand(ExecuteCommand command)
        {
            Assert.That(command.Command, Is.EqualTo("Kill Jar-Jar.cmd"));
        }
    }
}