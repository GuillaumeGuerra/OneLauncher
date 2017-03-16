using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using OneLauncher.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.Context;
using OneLauncher.Tests.Framework;
using OneLauncher.ViewModels;

namespace OneLauncher.Tests.ViewModels
{
    [TestFixture]
    public class SettingsViewViewModelTests
    {
        [Test]
        public void ShouldInitializeRepositoriesWhenLoaded()
        {
            var context =
                new OneLaunchContextMock().WithUserSettings(new UserSettings()
                {
                    Repositories =
                        new Dictionary<string, List<Repository>>()
                        {
                            {
                                "REPO1", new List<Repository>()
                                {
                                    new Repository() {Path = "Path1", Name = "Name1"},
                                    new Repository() {Path = "Path2", Name = "Name2"}
                                }
                            },
                            {
                                "REPO2", new List<Repository>()
                                {
                                    new Repository() {Path = "Path3", Name = "Name3"},
                                }
                            }
                        }
                });

            var vm = new SettingsViewViewModel()
            {
                Context = context
            };

            Assert.That(vm.Repositories, Has.Count.EqualTo(0));
            Assert.That(vm.SettingsChanged, Is.False);

            vm.LoadedCommand.Execute(null);

            WaitUtils.WaitFor(() => vm.Repositories.Count == 3, 10);

            // The repository dico should have been flatten
            Assert.That(vm.Repositories, Has.Count.EqualTo(3));
            AssertRepositoryViewModel(vm.Repositories[0], "REPO1", "Name1", "Path1");
            AssertRepositoryViewModel(vm.Repositories[1], "REPO1", "Name2", "Path2");
            AssertRepositoryViewModel(vm.Repositories[2], "REPO2", "Name3", "Path3");

            Assert.That(vm.SettingsChanged, Is.False);
        }

        [Test]
        public void ShouldInitializeLaunchersWhenLoaded()
        {
            var context =
                new OneLaunchContextMock().WithUserSettings(new UserSettings()
                {
                    ExcludedLauncherFilePaths = new List<string>()
                    {
                        "Path1",
                        "OldPath" // This one does not exist any more (it's not part of the discovered launchers)
                    }
                });

            var loader = new Mock<IConfigurationLoader>(MockBehavior.Strict);
            loader.Setup(mock => mock.DiscoverFiles(Environment.CurrentDirectory))
                .Returns(new[]
                {
                    new DiscoveredLauncher() { FilePath = "Path1" }, // Is in the exclusion list, so will be excluded
                    new DiscoveredLauncher() { FilePath = "Path2" } // Is not in the list, so will be active
                })
                .Verifiable();

            var vm = new SettingsViewViewModel()
            {
                Context = context,
                Loader = loader.Object
            };

            Assert.That(vm.Repositories, Has.Count.EqualTo(0));
            Assert.That(vm.SettingsChanged, Is.False);

            vm.LoadedCommand.Execute(null);

            WaitUtils.WaitFor(() => vm.Launchers.Count == 2, 10);

            // This file was excluded, according to the settings
            Assert.That(vm.Launchers[0].Path, Is.EqualTo("Path1"));
            Assert.That(vm.Launchers[0].Active, Is.False);

            // It was not in the user settings, so we'll suppose it's active
            Assert.That(vm.Launchers[1].Path, Is.EqualTo("Path2"));
            Assert.That(vm.Launchers[1].Active, Is.True);

            loader.VerifyAll();
        }

        [Test]
        public void ShouldSwitchSettingsChangedWhenCollectionIsUpdated()
        {
            var vm = new SettingsViewViewModel();

            vm.Launchers.Add(new LauncherViewModel());
            vm.Repositories.Add(new RepositoryViewModel());

            // Editing a repo should switch
            vm.SettingsChanged = false;
            vm.Repositories[0].Path = "Leia, you were soooo pretty";
            Assert.That(vm.SettingsChanged, Is.True);

            // Adding a repo should switch
            vm.SettingsChanged = false;
            vm.Repositories.Add(new RepositoryViewModel());
            Assert.That(vm.SettingsChanged, Is.True);

            // Removing a repo should switch
            vm.SettingsChanged = false;
            vm.Repositories.RemoveAt(0);
            Assert.That(vm.SettingsChanged, Is.True);

            // Excluding a launcher should switch
            vm.SettingsChanged = false;
            vm.Launchers[0].Active = !vm.Launchers[0].Active;
            Assert.That(vm.SettingsChanged, Is.True);
        }

        [Test]
        public void ShouldBuildProperDictionaryWhenSavingSettings()
        {
            var settings = new UserSettings() { Repositories = null };

            var context = new Mock<IOneLauncherContext>(MockBehavior.Strict);
            context.SetupGet(mock => mock.UserSettings)
                .Returns(settings)
                .Verifiable();
            context.Setup(mock => mock.SaveUserSettings())
                .Verifiable();

            var vm = new SettingsViewViewModel
            {
                Context = context.Object,
                Repositories = new TrulyObservableCollection<RepositoryViewModel>()
                {
                    new RepositoryViewModel() {Type = "REPO1", Name = "Name1", Path = "Path1"},
                    new RepositoryViewModel() {Type = "REPO2", Name = "Name2", Path = "Path2"}, // New repo, should be on its own in the dico
                    new RepositoryViewModel() {Type = "REPO1", Name = "Name3", Path = "Path3"} // Should be grouped with first row
                }
            };

            vm.SaveSettingsCommand.Execute(null);

            var actual = settings.Repositories;
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Has.Count.EqualTo(2));
            Assert.That(actual.ContainsKey("REPO1"), Is.True);

            var repo1 = actual["REPO1"];
            Assert.That(repo1, Is.Not.Null);
            Assert.That(repo1, Has.Count.EqualTo(2));
            Assert.That(repo1[0].Name, Is.EqualTo("Name1"));
            Assert.That(repo1[0].Path, Is.EqualTo("Path1"));
            Assert.That(repo1[1].Name, Is.EqualTo("Name3"));
            Assert.That(repo1[1].Path, Is.EqualTo("Path3"));

            var repo2 = actual["REPO2"];
            Assert.That(repo2, Is.Not.Null);
            Assert.That(repo2, Has.Count.EqualTo(1));
            Assert.That(repo2[0].Name, Is.EqualTo("Name2"));
            Assert.That(repo2[0].Path, Is.EqualTo("Path2"));

            context.VerifyAll();
        }

        [Test]
        public void ShouldExtractLauncherPathsWhenSavingSettings()
        {
            var settings = new UserSettings() { Repositories = null };

            var context = new Mock<IOneLauncherContext>(MockBehavior.Strict);
            context.SetupGet(mock => mock.UserSettings)
                .Returns(settings)
                .Verifiable();
            context.Setup(mock => mock.SaveUserSettings())
                .Verifiable();

            var vm = new SettingsViewViewModel
            {
                Context = context.Object,
                Launchers = new TrulyObservableCollection<LauncherViewModel>()
                {
                    new LauncherViewModel()
                    {
                        Active = true, // It's active, so we will not persist it in the settings
                        InnerDiscoveredLauncher = new DiscoveredLauncher(){FilePath = "Path1"}
                    },
                    new LauncherViewModel()
                    {
                        Active = false, // The user specified that it should be excluded, so we will store it in the settings
                        InnerDiscoveredLauncher = new DiscoveredLauncher(){FilePath = "Path2"}
                    }
                }
            };

            vm.SaveSettingsCommand.Execute(null);

            var actual = settings.ExcludedLauncherFilePaths;
            Assert.That(actual, Is.Not.Null);
            Assert.That(actual, Is.EquivalentTo(new[] { "Path2" }));

            context.VerifyAll();
        }

        private void AssertRepositoryViewModel(RepositoryViewModel vm, string type, string name, string path)
        {
            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.Type, Is.EqualTo(type));
            Assert.That(vm.Name, Is.EqualTo(name));
            Assert.That(vm.Path, Is.EqualTo(path));
        }
    }
}