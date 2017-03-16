using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using Infragistics.Controls.Menus;
using Moq;
using NUnit.Framework;
using OneLauncher.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;
using OneLauncher.Tests.Framework;
using OneLauncher.ViewModels;
using OneLauncher.Views;

namespace OneLauncher.Tests.ViewModels
{
    [TestFixture]
    public class OneLauncherViewModelTests
    {
        [Test]
        [Theory]
        [Apartment(ApartmentState.STA)]
        public void ShouldInitializeLaunchersWhenLoaded(bool defaultSettings)
        {
            var settings = new Mock<ISettingsView>(MockBehavior.Strict);
            // When the settings are the default ones, it means it's the first time the app is launched
            // We'll open the settings view, before the load is completed, to let the user defines his repositories location
            if (defaultSettings)
                settings
                    .Setup(mock => mock.ShowDialog())
                    .Returns(true)
                    .Verifiable();

            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterInstance(settings.Object).As<ISettingsView>();

            using (ContainerOverrider.Override(containerBuilder.Build()))
            {
                var launchersNodes = new[] { new LaunchersNode() { Header = "Header1" } };

                var loader = new Mock<IConfigurationLoader>(MockBehavior.Strict);
                loader
                    .Setup(mock => mock.LoadConfiguration(Environment.CurrentDirectory))
                    .Returns(launchersNodes)
                    .Verifiable();

                var context = new OneLaunchContextMock().WithUserSettings(new UserSettings() { IsDefaultSettings = defaultSettings });

                var vm = new OneLauncherViewModel()
                {
                    ConfigurationLoader = loader.Object,
                    Context = context
                };

                var builder = new Mock<IRadialMenuItemBuilder>(MockBehavior.Strict);
                var menuItem = new RadialMenuItem();
                builder
                    .Setup(mock => mock.BuildMenuItems(It.IsIn<IEnumerable<LaunchersNode>>(launchersNodes), It.IsIn(vm)))
                    .Returns(new[] { menuItem })
                    .Verifiable();

                vm.RadialMenuItemBuilder = builder.Object;

                // As long as the load is not completed, the VM should maintain the view closed
                Assert.That(vm.IsOpened, Is.False);

                var initialLaunchers = vm.Launchers;
                Assert.That(initialLaunchers, Has.Count.EqualTo(0));

                vm.LoadedCommand.Execute(null);

                WaitUtils.WaitFor(() => vm.IsOpened, 10, 1);

                Assert.That(vm.Launchers, Has.Count.EqualTo(1));
                Assert.That(vm.Launchers[0], Is.SameAs(menuItem));

                // Now it should be opened
                Assert.That(vm.IsOpened, Is.True);

                loader.VerifyAll();
                builder.VerifyAll();
                settings.VerifyAll();
            }
        }
    }

    [TestFixture]
    public class SettingsViewViewModelTests
    {
        [Test]
        public void ShouldInitializeSettingsWhenLoaded()
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

            // The repository dico should have been flatten
            Assert.That(vm.Repositories, Has.Count.EqualTo(3));
            AssertRepositoryViewModel(vm.Repositories[0], "REPO1", "Name1", "Path1");
            AssertRepositoryViewModel(vm.Repositories[1], "REPO1", "Name2", "Path2");
            AssertRepositoryViewModel(vm.Repositories[2], "REPO2", "Name3", "Path3");

            Assert.That(vm.SettingsChanged, Is.False);
        }

        [Test]
        public void ShouldSwitchSettingsChangedWhenCollectionIsUpdated()
        {
            var context = new OneLaunchContextMock().WithUserSettings(UserSettings.GetDefaultSettings());

            var vm = new SettingsViewViewModel()
            {
                Context = context
            };
            vm.LoadedCommand.Execute(null);

            Assert.That(vm.SettingsChanged, Is.False);
            Assert.That(vm.Repositories, Has.Count.GreaterThan(1));

            vm.Repositories[0].Path = "Leia, you were soooo pretty";

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

        private void AssertRepositoryViewModel(RepositoryViewModel vm, string type, string name, string path)
        {
            Assert.That(vm, Is.Not.Null);
            Assert.That(vm.Type, Is.EqualTo(type));
            Assert.That(vm.Name, Is.EqualTo(name));
            Assert.That(vm.Path, Is.EqualTo(path));
        }
    }
}
