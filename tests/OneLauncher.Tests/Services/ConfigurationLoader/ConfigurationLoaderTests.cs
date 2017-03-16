﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using OneLauncher.Commands.Commands.CopyFile;
using OneLauncher.Commands.Commands.ExecuteCommand;
using OneLauncher.Core.Commands;
using OneLauncher.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.Context;
using OneLauncher.Tests.Framework;
using ConfigLoader = OneLauncher.Services.ConfigurationLoader.ConfigurationLoader;

namespace OneLauncher.Tests.Services.ConfigurationLoader
{
    [TestFixture]
    public class ConfigurationLoaderTests
    {
        [Test]
        public void ShouldReadAllFilesLocatedInTheConfigurationAndUserSettingsDirectoriesWithTheRightPlugin()
        {
            using (var configurationDirectory = new TemporaryDirectory())
            using (var userSettingsDirectory = new TemporaryDirectory())
            {
                Directory.CreateDirectory($"{configurationDirectory.Location}\\Launchers");
                Directory.CreateDirectory($"{userSettingsDirectory.Location}\\OneLauncher");
                Directory.CreateDirectory($"{userSettingsDirectory.Location}\\OneLauncher\\Launchers");

                // First, we fill the configuration directory with some launcher files
                File.WriteAllText($"{configurationDirectory.Location}\\Launchers\\file1.xml", "");
                File.WriteAllText($"{configurationDirectory.Location}\\Launchers\\file2.json", "");

                // This one is not known by any plugin, it should be ignored
                File.WriteAllText($"{configurationDirectory.Location}\\Launchers\\file3.bin", "");

                // Now, we'll add one more file, in the user settings directory this time
                File.WriteAllText($"{userSettingsDirectory.Location}\\OneLauncher\\Launchers\\file4.txt", "");

                var xmlNode = new LaunchersNode() { Header = "Header1" };
                var jsonNode = new LaunchersNode() { Header = "Header2" };
                var txtNode = new LaunchersNode() { Header = "Header3" };

                var xmlLoader = ConfigureProcessMock($"{configurationDirectory.Location}\\Launchers\\file1.xml", xmlNode, ".xml");
                var jsonLoader = ConfigureProcessMock($"{configurationDirectory.Location}\\Launchers\\file2.json", jsonNode, ".json");
                var txtLoader = ConfigureProcessMock($"{userSettingsDirectory.Location}\\OneLauncher\\Launchers\\file4.txt", txtNode, ".txt");

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = new[] { xmlLoader.Object, jsonLoader.Object, txtLoader.Object },
                    Context = new OneLaunchContextMock().WithUserSettingsDirectory(userSettingsDirectory.Location)
                };
                var launchers = loader.LoadConfiguration(configurationDirectory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(3));
                Assert.That(launchers[0], Is.SameAs(xmlNode));
                Assert.That(launchers[1], Is.SameAs(jsonNode));
                Assert.That(launchers[2], Is.SameAs(txtNode));

                xmlLoader.VerifyAll();
                jsonLoader.VerifyAll();
                txtLoader.VerifyAll();
            }
        }

        [Test]
        public void ShouldNotLoadUserLaunchersIfTheUserLaunchersDirectoryDoesNotExist()
        {
            using (var configurationDirectory = new TemporaryDirectory())
            using (var userSettingsDirectory = new TemporaryDirectory())
            {
                Directory.CreateDirectory($"{configurationDirectory.Location}\\Launchers");
                // We don't create the Launchers directory in the user settings directory

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = Enumerable.Empty<ILauncherConfigurationProcessor>(),
                    Context = new OneLaunchContextMock().WithUserSettingsDirectory(userSettingsDirectory.Location)
                };
                var launchers = loader.LoadConfiguration(configurationDirectory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(0));
            }
        }

        [Test]
        public void ShouldMergeLauncherFilesWhenRepoHasTheSameName()
        {
            var firstNode = new LaunchersNode()
            {
                Header = "Repo1",
                Launchers = { new LauncherLink(), new LauncherLink() },
                SubGroups =
                {
                    new LaunchersNode()
                    {
                        Header = "SubGroup1",
                        Launchers = { new LauncherLink() },
                        SubGroups =
                        {
                            new LaunchersNode()
                            {
                                Header = "SubGroup1-1",
                                Launchers = {new LauncherLink()}
                            }
                        }

                    },
                    new LaunchersNode()
                    {
                        Header = "SubGroup2"
                    }
                }
            };

            var secondNode = new LaunchersNode()
            {
                Header = "Repo2",
                Launchers = { new LauncherLink(), new LauncherLink() },
                SubGroups =
                {
                    new LaunchersNode()
                    {
                        Header = "SubGroup1", // Same name than for the first node, but as the higher level differs, it won't be merged
                        Launchers = { new LauncherLink() },
                        SubGroups =
                        {
                            new LaunchersNode()
                            {
                                Header = "SubGroup1-1",
                                Launchers = { new LauncherLink() }
                            }
                        }

                    }
                }
            };


            var thirdNode = new LaunchersNode()
            {
                Header = "Repo1", // Same repo than the initial one, should be merged
                Launchers = { new LauncherLink() },
                SubGroups =
                {
                    new LaunchersNode()
                    {
                      Header  = "SubGroup3" // New node, won't be merged
                    },
                    new LaunchersNode()
                    {
                        Header = "SubGroup1", // Existing node, will be merged
                        Launchers = { new LauncherLink() },
                        SubGroups =
                        {
                            new LaunchersNode()
                            {
                                Header = "SubGroup1-2", // New subgroup, won't be merged
                                Launchers = {new LauncherLink()}
                            }
                        }
                    }
                }
            };


            var fourthNode = new LaunchersNode()
            {
                Header = "Repo1", // Again same repo than the initial one, should be merged
                Launchers = { new LauncherLink() }
            };

            using (var configurationDirectory = new TemporaryDirectory())
            using (var userSettingsDirectory = new TemporaryDirectory())
            {
                Directory.CreateDirectory($"{configurationDirectory.Location}\\Launchers");
                File.WriteAllText($"{configurationDirectory.Location}\\Launchers\\1.1", "");

                var loaderMock = CreateMockProcessor(firstNode, secondNode, thirdNode, fourthNode);

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = new[] { loaderMock.Object },
                    Context = new OneLaunchContextMock().WithUserSettingsDirectory(userSettingsDirectory.Location)
                };
                var launchers = loader.LoadConfiguration(configurationDirectory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(2));

                Assert.That(launchers[0].Header, Is.EqualTo("Repo1"));
                Assert.That(launchers[0].Launchers,
                    Is.EquivalentTo(new[] { firstNode.Launchers[0], firstNode.Launchers[1], thirdNode.Launchers[0], fourthNode.Launchers[0] }));

                Assert.That(launchers[0].SubGroups, Has.Count.EqualTo(3));
                Assert.That(launchers[0].SubGroups[0].Header, Is.EqualTo("SubGroup1"));
                Assert.That(launchers[0].SubGroups[0].Launchers,
                    Is.EquivalentTo(new[] { firstNode.SubGroups[0].Launchers[0], thirdNode.SubGroups[1].Launchers[0] }));
                Assert.That(launchers[0].SubGroups[0].SubGroups, Has.Count.EqualTo(2));
                Assert.That(launchers[0].SubGroups[0].SubGroups[0].Header, Is.EqualTo("SubGroup1-1"));
                Assert.That(launchers[0].SubGroups[0].SubGroups[0].Launchers,
                    Is.EquivalentTo(new[] { firstNode.SubGroups[0].SubGroups[0].Launchers[0] }));
                Assert.That(launchers[0].SubGroups[0].SubGroups[1].Header, Is.EqualTo("SubGroup1-2"));
                Assert.That(launchers[0].SubGroups[0].SubGroups[1].Launchers,
                    Is.EquivalentTo(new[] { thirdNode.SubGroups[1].SubGroups[0].Launchers[0] }));

                Assert.That(launchers[0].SubGroups[1].Header, Is.EqualTo("SubGroup2"));
                Assert.That(launchers[0].SubGroups[1].Launchers, Is.Null.Or.Empty);

                Assert.That(launchers[0].SubGroups[2].Header, Is.EqualTo("SubGroup3"));
                Assert.That(launchers[0].SubGroups[1].Launchers, Is.Null.Or.Empty);

                Assert.That(launchers[1].Header, Is.EqualTo("Repo2"));
                Assert.That(launchers[1].Launchers,
                    Is.EquivalentTo(new[] { secondNode.Launchers[0], secondNode.Launchers[1] }));
                Assert.That(launchers[1].SubGroups, Has.Count.EqualTo(1));
                Assert.That(launchers[1].SubGroups[0].Header, Is.EqualTo("SubGroup1"));
                Assert.That(launchers[1].SubGroups[0].Launchers,
                    Is.EquivalentTo(new[] { secondNode.SubGroups[0].Launchers[0] }));
                Assert.That(launchers[1].SubGroups[0].SubGroups, Has.Count.EqualTo(1));
                Assert.That(launchers[1].SubGroups[0].SubGroups[0].Header, Is.EqualTo("SubGroup1-1"));
                Assert.That(launchers[1].SubGroups[0].SubGroups[0].Launchers,
                    Is.EquivalentTo(new[] { secondNode.SubGroups[0].SubGroups[0].Launchers[0] }));
            }
        }

        [Test]
        public void LaunchersNodeShouldBeClonable()
        {
            var launchers = new LaunchersNode()
            {
                Header = "Repo1",
                Launchers =
                {
                    new LauncherLink()
                    {
                        Header = "Launcher1",
                        Commands = new List<LauncherCommand>()
                        {
                            new ExecuteCommand() { Command = "Command1" },
                            new FileCopierCommand() { SourceFilePath = "Source1", TargetFilePath = "Target1" }
                        }
                    }
                },
                SubGroups =
                {
                    new LaunchersNode()
                    {
                        Header = "SubGroup1",
                        Launchers =
                        {
                            new LauncherLink()
                            {
                                Header = "Launcher1",
                                Commands = new List<LauncherCommand>()
                                {
                                    new ExecuteCommand() { Command = "Command1" },
                                    new FileCopierCommand() { SourceFilePath = "Source1", TargetFilePath = "Target1" }
                                }
                            }
                        },
                    },
                    new LaunchersNode()
                    {
                        Header = "SubGroup2"
                    }
                }
            };

            // This test ensures that WCF serialization works, even though LauncherLink class contains a collection made of statically unknown subclasses
            // To overcome the issue, we have provided a KnownType collection for the LauncherCommand class
            launchers.DeepClone();
        }

        [Test]
        public void ShouldDiscoverAllLauncherFiles()
        {
            using (var directory = new TemporaryDirectory())
            {
                Directory.CreateDirectory($"{directory.Location}\\Launchers");

                File.WriteAllText($"{directory.Location}\\Launchers\\file1.xml", "");
                File.WriteAllText($"{directory.Location}\\Launchers\\file2.json", "");

                var xmlLoader = ConfigureProcessMock($"{directory.Location}\\Launchers\\file1.xml", new LaunchersNode(), ".xml");
                var jsonLoader = ConfigureProcessMock($"{directory.Location}\\Launchers\\file2.json", new LaunchersNode(), ".json");

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = new[] { xmlLoader.Object, jsonLoader.Object },
                    Context = new OneLaunchContextMock().WithUserSettingsDirectory("")
                };

                var launchers = loader.DiscoverFiles(directory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(2));
                Assert.That(launchers[0].FilePath, Is.EqualTo($"{directory.Location}\\Launchers\\file1.xml"));
                Assert.That(launchers[0].Processor, Is.SameAs(xmlLoader.Object));

                Assert.That(launchers[1].FilePath, Is.EqualTo($"{directory.Location}\\Launchers\\file2.json"));
                Assert.That(launchers[1].Processor, Is.SameAs(jsonLoader.Object));
            }
        }

        [Test]
        public void ShouldIgnoreLaunchersThatAreExcludedInTheUserSettings()
        {
            using (var directory = new TemporaryDirectory())
            {
                Directory.CreateDirectory($"{directory.Location}\\Launchers");

                File.WriteAllText($"{directory.Location}\\Launchers\\file1.xml", "");
                File.WriteAllText($"{directory.Location}\\Launchers\\file2.json", "");

                var xmlNode = new LaunchersNode() { Header = "Header1" };
                var jsonNode = new LaunchersNode() { Header = "Header2" };

                var xmlLoader = ConfigureProcessMock($"{directory.Location}\\Launchers\\file1.xml", xmlNode, ".xml");
                var jsonLoader = ConfigureProcessMock($"{directory.Location}\\Launchers\\file2.json", jsonNode, ".json");

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = new[] { xmlLoader.Object, jsonLoader.Object },
                    Context = new OneLaunchContextMock()
                        .WithExcludedLaunchers($"{directory.Location}\\Launchers\\file1.xml") // We exclude the first file
                        .WithUserSettingsDirectory(directory.Location)
                };

                var launchers = loader.LoadConfiguration(directory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(1));
                Assert.That(launchers[0], Is.SameAs(jsonNode));
            }
        }

        private Mock<ILauncherConfigurationProcessor> CreateMockProcessor(params LaunchersNode[] launchers)
        {
            var loader1 = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
            loader1.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();
            loader1.Setup(mock => mock.Load(It.IsAny<string>())).Returns(launchers).Verifiable();
            return loader1;
        }

        private Mock<ILauncherConfigurationProcessor> ConfigureProcessMock(string filePath, LaunchersNode returnedNode, string filePatterm)
        {
            var loader = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);

            loader
                .Setup(mock => mock.CanProcess(It.IsAny<string>()))
                .Returns<string>(s => s.EndsWith(filePatterm))
                .Verifiable();
            loader
                .Setup(mock => mock.Load(filePath))
                .Returns(new[] { returnedNode })
                .Verifiable();

            return loader;
        }
    }
}
