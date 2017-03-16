using System;
using System.Collections.Generic;
using System.Threading;
using Autofac;
using Infragistics.Controls.Menus;
using Moq;
using NUnit.Framework;
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
}
