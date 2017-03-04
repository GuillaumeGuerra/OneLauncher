using System;
using System.Linq;
using System.Xml.Serialization;
using NUnit.Framework;
using OneLauncher.Services.ConfigurationLoader.Xml;

namespace OneLauncher.Tests.Services.ConfigurationLoader.Xml
{
    [TestFixture]
    public class XmlLauncherConfigurationReaderTests
    {
        [Test]
        public void ShouldReadAllRowsProperlyWhenLoadingXmlFile()
        {
            var actual = new XmlLauncherConfigurationReader().LoadFile("Data/LaunchersTest1.xml");
            Assert.That(actual.RepoType, Is.EqualTo("Kirk you stink"));

            Assert.That(actual.GenericTemplate, Is.Not.Null);
            Assert.That(actual.GenericTemplate.Header, Is.Null); // No header for the template

            Assert.That(actual.GenericTemplate.Launchers, Is.Not.Null);
            Assert.That(actual.GenericTemplate.Launchers, Has.Count.EqualTo(1));

            AssertSingleCommandLauncher("Empire", "[ROOT]/Rebels/DeathStar/Fire.cmd", actual.GenericTemplate.Launchers[0]);

            Assert.That(actual.GenericTemplate.SubGroups, Is.Not.Null);
            Assert.That(actual.GenericTemplate.SubGroups, Has.Count.EqualTo(2));

            var firstGroup = actual.GenericTemplate.SubGroups[0];
            Assert.That(firstGroup.Header, Is.EqualTo("Solutions"));

            Assert.That(firstGroup.Launchers, Is.Not.Null);
            Assert.That(firstGroup.Launchers, Has.Count.EqualTo(2));

            Assert.That(firstGroup.Launchers[0], Is.Not.Null);
            Assert.That(firstGroup.Launchers[0].Header, Is.EqualTo("Base.sln"));
            Assert.That(firstGroup.Launchers[0].Commands, Has.Count.EqualTo(3));
            // First, the Execute command
            Assert.That(((XmlExecuteCommand)firstGroup.Launchers[0].Commands[0]).Command, Is.EqualTo("[ROOT]/Rebels/Yavin/base.sln"));
            AssertXmlXPathReplacerCommand("[ROOT]/assembly.dll.config", "configuration/appSettings[@name='who's the best jedi ?']", "yoda", firstGroup.Launchers[0].Commands[1]);
            AssertXmlFileCopierCommand("[ROOT]/somewhere/assembly.dll.config", "[ROOT]/somewhere else/assembly.dll.config", firstGroup.Launchers[0].Commands[2]);

            AssertSingleCommandLauncher("Padawan.sln", "[ROOT]/Jedis/padawan.sln", firstGroup.Launchers[1]);

            Assert.That(firstGroup.SubGroups, Is.Not.Null);
            Assert.That(firstGroup.SubGroups, Has.Count.EqualTo(0));

            var secondGroup = actual.GenericTemplate.SubGroups[1];
            Assert.That(secondGroup.Header, Is.EqualTo("Launchers"));

            Assert.That(secondGroup.Launchers, Is.Not.Null);
            Assert.That(secondGroup.Launchers, Has.Count.EqualTo(1));
            AssertSingleCommandLauncher("Rebellion", "[ROOT]/Rebels/Yavin/bin/debug/Start rebellion.cmd", secondGroup.Launchers[0]);

            Assert.That(secondGroup.SubGroups, Is.Not.Null);
            Assert.That(secondGroup.SubGroups, Has.Count.EqualTo(1));

            var subGroup = secondGroup.SubGroups[0];
            Assert.That(subGroup.Header, Is.EqualTo("Big ass launchers"));

            Assert.That(subGroup.Launchers, Has.Count.EqualTo(1));
            AssertSingleCommandLauncher("Jar-Jar you stink", "Kick Jar-Jar.ps1", subGroup.Launchers[0]);
        }

        [Test]
        public void ShouldThrowWhenFilePathIsUnknown()
        {
            var service = new XmlLauncherConfigurationReader();
            Assert.That(() => service.LoadFile("E:/unknown_path"), Throws.Exception.TypeOf<InvalidOperationException>().And.Message.EqualTo("Unknow file path [E:/unknown_path]"));
        }

        [Test]
        public void AllTypeOfCommandsShouldBeDecoratedWithAnXmlAttribute()
        {
            var commands =
                typeof(XmlLauncherCommand).Assembly.GetTypes()
                    .Where(t => t.BaseType != null && t.BaseType == typeof(XmlLauncherCommand))
                    .ToList();

            foreach (var command in commands)
            {
                Assert.That(command.GetCustomAttributes(typeof(XmlNameAttribute), false), Has.Length.EqualTo(1), $"Missing XmlNameAttribute for type [{command.FullName}]");

                var attribute = command.GetCustomAttributes(typeof(XmlNameAttribute), false)[0] as XmlNameAttribute;
                Assert.That(attribute.Name, Is.Not.Null.And.Not.Empty, $"Null or empty name for XmlNameAttribute on type [{command.FullName}]");
            }
        }

        private void AssertSingleCommandLauncher(string header, string command, XmlLauncherLink launcher)
        {
            Assert.That(launcher, Is.Not.Null);
            Assert.That(launcher.Header, Is.EqualTo(header));
            Assert.That(launcher.Commands, Has.Count.EqualTo(1));
            Assert.That(((XmlExecuteCommand)launcher.Commands[0]).Command, Is.EqualTo(command));
        }

        private void AssertXmlXPathReplacerCommand(string filePath, string xpath, string value, XmlLauncherCommand command)
        {
            var xpathCommand = (XmlXPathReplacerCommand)command;

            Assert.That(xpathCommand.XPath, Is.EqualTo(xpath));
            Assert.That(xpathCommand.Value, Is.EqualTo(value));
            Assert.That(xpathCommand.FilePath, Is.EqualTo(filePath));
        }

        private void AssertXmlFileCopierCommand(string sourceFilePath, string targetFilePath, XmlLauncherCommand command)
        {
            var xpathCommand = (XmlFileCopierCommand)command;

            Assert.That(xpathCommand.SourceFilePath, Is.EqualTo(sourceFilePath));
            Assert.That(xpathCommand.TargetFilePath, Is.EqualTo(targetFilePath));
        }
    }

    [TestFixture]
    public class XmlCopierCommandTests
    {
        [Test]
        public void ShouldReadConfigurationFromXml()
        {
            var xml = @"<File SourceFilePath=""jar-jar"" TargetFilePath=""trash"" />";
            var outerXml = @"
<Configuration>
    <GenericTemplate>
        <Launcher>" + 
            xml + 
        @"</Launcher>
    </GenericTemplate>
</Configuration>";

            // TODO complete the generic UT
            var actual = new XmlLauncherConfigurationReader().LoadFile("Data/LaunchersTest1.xml");
        }
    }
}
