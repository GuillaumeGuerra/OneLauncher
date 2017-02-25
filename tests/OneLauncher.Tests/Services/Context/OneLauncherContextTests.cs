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
        public void TestThatANewUserSettingFileIsCreatedWhenItIsSavedForTheFirstTime()
        {
            using (var directory = new TemporaryDirectory())
            {
                var context = GetContext(directory);

                // Even though there is no user settings yet, default one will be given in the context
                Assert.That(context.UserSettings, Is.Not.Null);
                Assert.That(context.UserSettings.Repositories, Has.Count.EqualTo(0));

                // Let's edit them
                context.UserSettings.Repositories.Add("MyAss", new List<string>() { "Is", "Not", "That", "Big" });

                // Now we save them : a file should have been created
                context.SaveUserSettings();
                
                Assert.That(File.Exists(Path.Combine(directory.Location, "UserSettings.json")), Is.True);
                Assert.That(File.ReadAllText(Path.Combine(directory.Location, "UserSettings.json")), Has.Length.GreaterThan(5));

                // This time, when the context is loaded, we should get the new version of the settings
                Assert.That(GetContext(directory).UserSettings.Repositories, Has.Count.EqualTo(1));
            }
        }

        [Test]
        public void TestThatUserSettingsAreLoadedFromTheJsonFileWhenItExists()
        {
            var context = new OneLauncherContext();

            // There was a settings file, it should have been loaded, and replace the default values
            Assert.That(context.UserSettings.Repositories, Has.Count.EqualTo(1));
            Assert.That(context.UserSettings.Repositories["XONE"], Is.EquivalentTo(new[] { "path1", "path2" }));
            Assert.That(context.UserSettings.SettingsVersion, Is.EqualTo("42.0"));

            // By resaving the settings, we'll test that we can overwrite settings when they are already available

            context.UserSettings.Repositories["XONE"].Add("path3");
            context.SaveUserSettings();

            Assert.That(new OneLauncherContext().UserSettings.Repositories["XONE"], Has.Count.EqualTo(3));
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
