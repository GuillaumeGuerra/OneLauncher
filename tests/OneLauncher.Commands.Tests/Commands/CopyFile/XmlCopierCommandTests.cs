using NUnit.Framework;
using OneLauncher.Commands.Commands.CopyFile;

namespace OneLauncher.Commands.Tests.Commands.CopyFile
{
    [TestFixture]
    public class XmlCopierCommandTests: CommonXmlCommandTests<XmlFileCopierCommand, FileCopierCommand>
    {
        protected override string GetTemplateXml()
        {
            return @"<CopyFile SourceFilePath=""Jar-Jar.Binks"" TargetFilePath=""Trash"" />";
        }

        protected override void AssertXmlCommandDeserializedFromXml(XmlFileCopierCommand xmlCommand)
        {
            Assert.That(xmlCommand.SourceFilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(xmlCommand.TargetFilePath, Is.EqualTo("Trash")); // Jar-Jar belongs to the trash
        }

        protected override XmlFileCopierCommand GetTemplateXmlCommand()
        {
            return new XmlFileCopierCommand()
            {
                SourceFilePath = "Jar-Jar.Binks",
                TargetFilePath = "Trash"
            };
        }

        protected override void AssertCommandExtractedFromXmlCommand(FileCopierCommand command)
        {
            Assert.That(command.SourceFilePath, Is.EqualTo("Jar-Jar.Binks"));
            Assert.That(command.TargetFilePath, Is.EqualTo("Trash")); // Jar-Jar belongs to the trash
        }
    }
}