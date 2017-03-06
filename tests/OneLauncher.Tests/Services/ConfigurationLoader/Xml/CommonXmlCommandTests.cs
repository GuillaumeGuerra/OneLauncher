using System.IO;
using NUnit.Framework;
using OneLauncher.Core.Commands;
using OneLauncher.Services.ConfigurationLoader.Xml;
using OneLauncher.Tests.Framework;

namespace OneLauncher.Tests.Services.ConfigurationLoader.Xml
{
    [TestFixture]
    public abstract class CommonXmlCommandTests<TXmlCommand, TCommand>
        where TXmlCommand : XmlLauncherCommand
        where TCommand : LauncherCommand
    {
        [Test]
        public void ShouldReadConfigurationFromXml()
        {
            using (var directory = new TemporaryDirectory())
            {
                var xml = GetTemplateXml();
                var outerXml =
                    @"<Configuration><GenericTemplate><Launchers><Launcher><Commands>" +
                    xml +
                    @"</Commands></Launcher></Launchers></GenericTemplate></Configuration>";

                var path = $"{directory.Location}/launchers.xml";
                File.WriteAllText(path, outerXml);

                var actual = new XmlLauncherConfigurationReader().LoadFile(path);

                Assert.That(actual.GenericTemplate.Launchers, Has.Count.EqualTo(1));
                Assert.That(actual.GenericTemplate.Launchers[0].Commands, Has.Count.EqualTo(1));
                Assert.That(actual.GenericTemplate.Launchers[0].Commands[0], Is.InstanceOf<TXmlCommand>());

                var command = actual.GenericTemplate.Launchers[0].Commands[0] as TXmlCommand;
                AssertXmlCommandDeserializedFromXml(command);
            }
        }

        [Test]
        public void ShouldExtractSpecificLauncherCommandFromXmlCommand()
        {
            var xmlCommand = GetTemplateXmlCommand();

            var command = xmlCommand.ToCommand("");
            Assert.That(command, Is.InstanceOf<TCommand>());

            var fileCopierCommand = command as TCommand;
            AssertCommandExtractedFromXmlCommand(fileCopierCommand);
        }

        protected abstract void AssertXmlCommandDeserializedFromXml(TXmlCommand xmlCommand);
        protected abstract string GetTemplateXml();
        protected abstract TXmlCommand GetTemplateXmlCommand();
        protected abstract void AssertCommandExtractedFromXmlCommand(TCommand command);
    }
}