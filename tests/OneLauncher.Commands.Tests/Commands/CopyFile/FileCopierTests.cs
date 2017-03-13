using System.IO;
using NUnit.Framework;
using OneLauncher.Commands.Commands.CopyFile;
using OneLauncher.Tests.Framework;

namespace OneLauncher.Commands.Tests.Commands.CopyFile
{
    [TestFixture]
    public class FileCopierTests : CommonCommandLauncherTests<FileCopier, FileCopierCommand>
    {
        [Test]
        public void ShouldCopyFileWhenSourceAndTargetPathsAreValid()
        {
            using (var directory = new TemporaryDirectory())
            {
                // Just to be sure, the file is not supposed to be here already ...
                Assert.That(File.Exists($"{directory.Location}/OmniLauncher.Tests.dll.config"), Is.False);

                new FileCopier().Execute(new FileCopierCommand()
                {
                    SourceFilePath = "Data/OmniLauncher.Tests.dll.config",
                    TargetFilePath = $"{directory.Location}/OmniLauncher.Tests.dll.config"
                });

                // Now it should be
                Assert.That(File.Exists($"{directory.Location}/OmniLauncher.Tests.dll.config"), Is.True);

                // And it should be strictly identical to the source file
                Assert.That(File.ReadAllText("Data/OmniLauncher.Tests.dll.config"),
                    Is.EqualTo(File.ReadAllText($"{directory.Location}/OmniLauncher.Tests.dll.config")));

                // Also, it should not fail in case the file already exists
                Assert.That(() => new FileCopier().Execute(new FileCopierCommand()
                    {
                        SourceFilePath = "Data/OmniLauncher.Tests.dll.config",
                        TargetFilePath = $"{directory.Location}/OmniLauncher.Tests.dll.config"
                    }), Throws.Nothing
                );
            }
        }

        [Test]
        public void ShouldThrowWhenSourceFilePathCanNotBeFound()
        {
            using (var directory = new TemporaryDirectory())
            {
                Assert.That(
                    () =>
                        new FileCopier().Execute(new FileCopierCommand()
                        {
                            SourceFilePath = "//john/doe",
                            TargetFilePath = $"{directory.Location}/test.config"
                        }),
                    Throws.Exception.InstanceOf<FileNotFoundException>()
                        .With.Message.EqualTo("Unable to find file to copy [//john/doe]"));
            }
        }

        [Test]
        public void ShouldThrowWhenTargetDirectoryCanNotBeFound()
        {
            Assert.That(
                () =>
                    new FileCopier().Execute(new FileCopierCommand()
                    {
                        SourceFilePath = "Data/OmniLauncher.Tests.dll.config",
                        TargetFilePath = "//john/doe/test.config"
                    }),
                Throws.Exception.InstanceOf<FileNotFoundException>()
                    .With.Message.EqualTo(@"Unable to find target directory to copy to [\\john\doe]"));
        }
    }
}