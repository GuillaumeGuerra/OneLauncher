using NUnit.Framework;
using OneLauncher.Commands.Commands.ExecuteCommand;

namespace OneLauncher.Commands.Tests.Commands.ExecuteCommand
{
    [TestFixture]
    public class XmlExecuteCommandTests : CommonXmlCommandTests<XmlExecuteCommand, OneLauncher.Commands.Commands.ExecuteCommand.ExecuteCommand>
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

        protected override void AssertCommandExtractedFromXmlCommand(OneLauncher.Commands.Commands.ExecuteCommand.ExecuteCommand command)
        {
            Assert.That(command.Command, Is.EqualTo("Kill Jar-Jar.cmd"));
        }
    }
}