﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Moq;
using NUnit.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Tests.Framework;
using ConfigLoader = OneLauncher.Services.ConfigurationLoader.ConfigurationLoader;

namespace OneLauncher.Tests.Services.ConfigurationLoader
{
    [TestFixture]
    public class ConfigurationLoaderTests
    {
        [Test]
        public void ShouldReadAllFilesLocatedInTheConfigurationDirectoryWithTheRightPlugin()
        {
            using (var directory = new TemporaryDirectory())
            {
                File.WriteAllText($"{directory.Location}\\file1.xml", "");
                File.WriteAllText($"{directory.Location}\\file2.json", "");

                // This one is not known by any plugin, it should be ignored
                File.WriteAllText($"{directory.Location}\\file3.bin", "");

                var firstNode = new LaunchersNode() { Header = "Header1" };
                var secondNode = new LaunchersNode() { Header = "Header2" };

                var xmlLoader = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
                xmlLoader.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                    .Returns<string>(s => s.EndsWith(".xml"))
                    .Verifiable();
                xmlLoader.Setup(mock => mock.Load($"{directory.Location}\\file1.xml")).Returns(new[] { firstNode }).Verifiable();

                var jsonLoader = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
                jsonLoader.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                    .Returns<string>(s => s.EndsWith(".json"))
                    .Verifiable();
                jsonLoader.Setup(mock => mock.Load($"{directory.Location}\\file2.json")).Returns(new[] { secondNode }).Verifiable();

                var loader = new ConfigLoader() { AllConfigurationProcessors = new[] { xmlLoader.Object, jsonLoader.Object } };
                var launchers = loader.LoadConfiguration(directory.Location).ToList();

                Assert.That(launchers, Has.Count.EqualTo(2));
                Assert.That(launchers[0], Is.SameAs(firstNode));
                Assert.That(launchers[1], Is.SameAs(secondNode));

                xmlLoader.VerifyAll();
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

            using (var directory = new TemporaryDirectory())
            {
                File.WriteAllText($"{directory.Location}\\1.1", "");

                var loaderMock = CreateMockProcessor(firstNode, secondNode, thirdNode, fourthNode);

                var loader = new ConfigLoader()
                {
                    AllConfigurationProcessors = new[] { loaderMock.Object }
                };
                var launchers = loader.LoadConfiguration(directory.Location).ToList();

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

        private Mock<ILauncherConfigurationProcessor> CreateMockProcessor(params LaunchersNode[] launchers)
        {
            var loader1 = new Mock<ILauncherConfigurationProcessor>(MockBehavior.Strict);
            loader1.Setup(mock => mock.CanProcess(It.IsAny<string>()))
                .Returns(true)
                .Verifiable();
            loader1.Setup(mock => mock.Load(It.IsAny<string>())).Returns(launchers).Verifiable();
            return loader1;
        }
    }
}
