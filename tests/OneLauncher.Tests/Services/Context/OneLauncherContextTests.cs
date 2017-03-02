using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using OneLauncher.Services.Context;
using OneLauncher.Tests.Framework;

namespace OneLauncher.Tests.Services.Context
{
    [TestFixture]
    public class OneLauncherContextTests
    {
        [Test]
        public void TestThatApplicationSettingsAreExtractedFromTheAppConfigFile()
        {
            var context = new OneLauncherContext();

            Assert.That(context.ApplicationSettings, Is.Not.Null);
            Assert.That(context.ApplicationSettings.UserSettingsDirectory, Is.EqualTo("Data"));
            Assert.That(context.ApplicationSettings.UserSettingsFileName, Is.EqualTo("UserSettingsTest.json"));
        }

        [Test]
        public void TestThatANewUserSettingFileIsCreatedWhenTheContextIsLoadedForTheFirstTime()
        {
            using (var directory = new TemporaryDirectory())
            {
                var userSettingsPath = $"{directory.Location}/OneLauncher/UserSettings.json";

                // So far, obviously, the user settings file does not exist yet
                Assert.That(File.Exists(userSettingsPath), Is.False);

                var context = GetContext(directory);

                // Even though there was no user settings yet, default one will be given in the context
                Assert.That(context.UserSettings, Is.Not.Null);
                Assert.That(context.UserSettings.Repositories, Has.Count.GreaterThan(0));
                Assert.That(context.UserSettings.IsDefaultSettings, Is.True);

                // And it should have been created in the directory
                Assert.That(File.Exists(userSettingsPath), Is.True);

                var defaultContent = File.ReadAllText(userSettingsPath);

                // Let's edit them
                context.UserSettings.Repositories.Add("MyAss", new List<Repository>() { new Repository() { Name = "IsNotThatBig" } });

                // Now we save them : the file should have been updated
                context.SaveUserSettings();

                // The saved file should be different, obviously
                Assert.That(File.ReadAllText(userSettingsPath), Is.Not.EqualTo(defaultContent));

                // And the settings no longer by default
                Assert.That(context.UserSettings.IsDefaultSettings, Is.False);

                // This time, when the context is loaded, we should get the new version of the settings
                Assert.That(GetContext(directory).UserSettings.Repositories, Has.Count.EqualTo(2));
            }
        }

        [Test]
        public void TestThatUserSettingsAreLoadedFromTheJsonFileWhenItExists()
        {
            using (var directory = new TemporaryDirectory())
            {
                // First, we'll copy a settings file
                Directory.CreateDirectory($"{directory.Location}/OneLauncher");
                File.Copy("Services/Context/Data/UserSettings.json", $"{directory.Location}/OneLauncher/UserSettings.json");

                var context = GetContext(directory);

                // There was a settings file, it should have been loaded, and replace the default values
                Assert.That(context.UserSettings.Repositories, Has.Count.EqualTo(1));

                var repos = context.UserSettings.Repositories["XONE"];
                Assert.That(repos, Has.Count.EqualTo(2));

                Assert.That(repos[0].Name, Is.EqualTo("First"));
                Assert.That(repos[0].Path, Is.EqualTo("path1"));

                Assert.That(repos[1].Name, Is.EqualTo("Second"));
                Assert.That(repos[1].Path, Is.EqualTo("path2"));

                Assert.That(context.UserSettings.SettingsVersion, Is.EqualTo("42.0"));

                Assert.That(context.UserSettings.IsDefaultSettings, Is.False);

                // By resaving the settings, we'll test that we can overwrite settings when they are already available

                repos.Add(new Repository() { Name = "Third", Path = "path3" });
                context.SaveUserSettings();

                Assert.That(GetContext(directory).UserSettings.Repositories["XONE"], Has.Count.EqualTo(3));
            }
        }

        private OneLauncherContext GetContext(TemporaryDirectory directory)
        {
            var context = new OneLauncherContext();

            context.ApplicationSettings.UserSettingsDirectory = directory.Location;
            context.ApplicationSettings.UserSettingsFileName = "UserSettings.json";

            return context;
        }
    }
}
