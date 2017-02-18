using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OneLauncher.Services.CommandLauncher;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.ConfigurationLoader.Xml;

namespace OneLauncher.Tests
{
    [TestFixture]
    public class LauncherConfigurationProcessorTests
    {
        [Test]
        public void ShouldReplaceRootTokenWithRootDirectory()
        {
            var reader = new Mock<IXmlLauncherConfigurationReader>(MockBehavior.Strict);
            reader.Setup(mock => mock.LoadFile("")).Returns(GetTemplateConfiguration()).Verifiable();

            var processor = new XmlLauncherConfigurationProcessor() { ConfigurationReader = reader.Object };
            var actual = processor.Load("");

            Assert.That(actual.SubGroups, Is.Not.Null);
            Assert.That(actual.SubGroups.Count, Is.EqualTo(2));

            AssertNode(actual.SubGroups[0], "D:/Jar-Jar You Stink", "Jar-Jar");
            AssertNode(actual.SubGroups[1], "E:/Star Trek/Kirk_you_suck", "Kirk");
        }

        [TestCase("root/path/with/slashes", "[ROOT]/end/of/path", "root/path/with/slashes/end/of/path",
            Description = "Genuine case")]
        [TestCase("root/path/with/slashes/", "[ROOT]/end/of/path", "root/path/with/slashes/end/of/path",
            Description = "Extra / after the root")]
        [TestCase(@"root\path\with\backslashes", @"[ROOT]\end\of\path", @"root\path\with\backslashes\end\of\path",
            Description = "Genuine case")]
        [TestCase(@"root\path\with\backslashes\", @"[ROOT]\end\of\path", @"root\path\with\backslashes\end\of\path",
            Description = @"Extra \ after the root")]
        public void ShouldProcessSlashAndBackSlashWhenReplacingRootToken(string path, string command, string expected)
        {
            var processor = new XmlLauncherConfigurationProcessor();
            var actual = processor.ProcessLauncherLink(new XmlLauncherRootDirectory()
            {
                Path = path
            },
                new XmlLauncherLink()
                {
                    Commands = new List<XmlLauncherCommand>()
                    {
                        new XmlExecuteCommand()
                        {
                            Command = command
                        },
                        new XmlXPathReplacerCommand()
                        {
                            FilePath = command
                        }
                    }
                });

            Assert.That(((ExecuteCommand)actual.Commands[0]).Command, Is.EqualTo(expected));
            Assert.That(((XPathReplacerCommand)actual.Commands[1]).FilePath, Is.EqualTo(expected));
        }

        [TestCase("test.xml", true)]
        [TestCase("test.json", false)]
        [TestCase("meskouyan.ski", false)]
        public void ShouldOnlyProcessXmlFiles(string path, bool shouldProcess)
        {
            Assert.That(new XmlLauncherConfigurationProcessor().CanProcess(path), Is.EqualTo(shouldProcess));
        }

        private void AssertNode(LaunchersNode rootGroup, string rootDirectory, string expectedHeader)
        {
            Assert.That(rootGroup.Header, Is.EqualTo(expectedHeader));

            Assert.That(rootGroup.SubGroups, Is.Not.Null);
            Assert.That(rootGroup.SubGroups.Count, Is.EqualTo(2));

            Assert.That(rootGroup.SubGroups[0], Is.Not.Null);
            Assert.That(rootGroup.SubGroups[0].Header, Is.EqualTo("Solutions"));

            Assert.That(rootGroup.SubGroups[0].Launchers, Is.Not.Null);
            Assert.That(rootGroup.SubGroups[0].Launchers.Count, Is.EqualTo(2));

            Assert.That(rootGroup.SubGroups[0].Launchers[0].Header, Is.EqualTo("Base.sln"));
            Assert.That(rootGroup.SubGroups[0].Launchers[0].Commands, Has.Count.EqualTo(3));
            AssertExecuteCommand(rootGroup.SubGroups[0].Launchers[0].Commands[0], rootDirectory + "/Rebels/Yavin/base.sln");

            Assert.That(rootGroup.SubGroups[0].Launchers[0].Commands[1], Is.InstanceOf<XPathReplacerCommand>());
            Assert.That(((XPathReplacerCommand)rootGroup.SubGroups[0].Launchers[0].Commands[1]).FilePath, Is.EqualTo(rootDirectory + "/assembly.dll.config"));
            Assert.That(((XPathReplacerCommand)rootGroup.SubGroups[0].Launchers[0].Commands[1]).XPath, Is.EqualTo("configuration/appSettings[@name='who's the best jedi ?']"));
            Assert.That(((XPathReplacerCommand)rootGroup.SubGroups[0].Launchers[0].Commands[1]).Value, Is.EqualTo("yoda"));

            Assert.That(rootGroup.SubGroups[0].Launchers[0].Commands[2], Is.InstanceOf<FileCopierCommand>());
            Assert.That(((FileCopierCommand)rootGroup.SubGroups[0].Launchers[0].Commands[2]).SourceFilePath, Is.EqualTo(rootDirectory + "/somewhere/assembly.dll.config"));
            Assert.That(((FileCopierCommand)rootGroup.SubGroups[0].Launchers[0].Commands[2]).TargetFilePath, Is.EqualTo(rootDirectory + "/somewhere else/assembly.dll.config"));

            Assert.That(rootGroup.SubGroups[0].Launchers[1].Header, Is.EqualTo("Padawan.sln"));
            AssertExecuteCommand(rootGroup.SubGroups[0].Launchers[1].Commands[0], rootDirectory + "/Jedis/padawan.sln");

            Assert.That(rootGroup.SubGroups[1], Is.Not.Null);
            Assert.That(rootGroup.SubGroups[1].Header, Is.EqualTo("Launchers"));

            Assert.That(rootGroup.SubGroups[1].Launchers, Is.Not.Null);
            Assert.That(rootGroup.SubGroups[1].Launchers.Count, Is.EqualTo(1));

            Assert.That(rootGroup.SubGroups[1].Launchers[0].Header, Is.EqualTo("Rebellion"));
            AssertExecuteCommand(rootGroup.SubGroups[1].Launchers[0].Commands[0], rootDirectory + "/Rebels/Yavin/bin/debug/Start rebellion.cmd");
        }

        private XmlLauncherConfiguration GetTemplateConfiguration()
        {
            var configuration = new XmlLauncherConfiguration
            {
                RootDirectories = new List<XmlLauncherRootDirectory>()
                {
                    new XmlLauncherRootDirectory()
                    {
                        Header = "Jar-Jar",
                        Path = "D:/Jar-Jar You Stink"
                    },
                    new XmlLauncherRootDirectory()
                    {
                        Header = "Kirk",
                        Path = "E:/Star Trek/Kirk_you_suck"
                    }
                },
                GenericTemplate = new XmlLauncherNode()
                {
                    SubGroups = new List<XmlLauncherNode>()
                    {
                        new XmlLauncherNode()
                        {
                            Header = "Solutions",
                            Launchers = new List<XmlLauncherLink>()
                            {
                                new XmlLauncherLink()
                                {
                                    Header = "Base.sln",
                                    Commands = new List<XmlLauncherCommand>()
                                    {
                                        new XmlExecuteCommand()
                                        {
                                            Command = "[ROOT]/Rebels/Yavin/base.sln"
                                        },
                                        new XmlXPathReplacerCommand()
                                        {
                                            FilePath = "[ROOT]/assembly.dll.config",
                                            XPath = "configuration/appSettings[@name='who's the best jedi ?']",
                                            Value = "yoda"
                                        },
                                        new XmlFileCopierCommand()
                                        {
                                            SourceFilePath = "[ROOT]/somewhere/assembly.dll.config",
                                            TargetFilePath = "[ROOT]/somewhere else/assembly.dll.config"
                                        }
                                    }
                                },
                                new XmlLauncherLink()
                                {
                                    Header = "Padawan.sln",
                                    Commands = new List<XmlLauncherCommand>()
                                    {
                                        new XmlExecuteCommand()
                                        {
                                            Command = "[ROOT]/Jedis/padawan.sln"
                                        }
                                    }
                                }
                            }
                        },
                        new XmlLauncherNode()
                        {
                            Header = "Launchers",
                            Launchers = new List<XmlLauncherLink>()
                            {
                                new XmlLauncherLink()
                                {
                                    Header = "Rebellion",
                                    Commands = new List<XmlLauncherCommand>()
                                    {
                                        new XmlExecuteCommand()
                                        {
                                            Command = "[ROOT]/Rebels/Yavin/bin/debug/Start rebellion.cmd"
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            return configuration;
        }

        private void AssertExecuteCommand(LauncherCommand launcher, string command)
        {
            Assert.That(launcher, Is.InstanceOf<ExecuteCommand>());
            var execute = launcher as ExecuteCommand;
            Assert.That(execute.Command, Is.EqualTo(command));
        }
    }
}
