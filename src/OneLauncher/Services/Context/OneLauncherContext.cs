using System;
using System.Configuration;
using System.IO;
using Autofac.AttributeExtensions;
using Newtonsoft.Json;

namespace OneLauncher.Services.Context
{
    [SingleInstance]
    public class OneLauncherContext : IOneLauncherContext
    {
        private readonly Lazy<UserSettings> _userSettings;

        public ApplicationSettings ApplicationSettings { get; }
        public UserSettings UserSettings { get { return _userSettings.Value; } }

        public OneLauncherContext()
        {
            ApplicationSettings = new ApplicationSettings()
            {
                UserSettingsFileName = ConfigurationManager.AppSettings["userSettingsFileName"],
                UserSettingsDirectory = ConfigurationManager.AppSettings["userSettingsDirectory"]
            };
            _userSettings = new Lazy<UserSettings>(LoadUserSettings);
        }

        public void SaveUserSettings()
        {
            var json = JsonConvert.SerializeObject(UserSettings, Formatting.Indented);
            File.WriteAllText(GetUserSettingsFilePath(), json);

            UserSettings.IsDefaultSettings = false;
        }

        private UserSettings LoadUserSettings()
        {
            var userSettingsFilePath = GetUserSettingsFilePath();

            if (File.Exists(userSettingsFilePath))
            {
                var loadedSettings = JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(userSettingsFilePath));
                loadedSettings.IsDefaultSettings = false;
                return loadedSettings;
            }

            if (!Directory.Exists(GetUserSettingsDirectoryPath()))
                Directory.CreateDirectory(GetUserSettingsDirectoryPath());

            // In case the file doesn't exist, we'll create it, with a default value
            var json = JsonConvert.SerializeObject(UserSettings.GetDefaultSettings(), Formatting.Indented);
            File.WriteAllText(userSettingsFilePath, json);

            return UserSettings.GetDefaultSettings();
        }

        private string GetUserSettingsDirectoryPath()
        {
            return Path.Combine(ApplicationSettings.UserSettingsDirectory, "OneLauncher");
        }

        private string GetUserSettingsFilePath()
        {
            return Path.Combine(GetUserSettingsDirectoryPath(), ApplicationSettings.UserSettingsFileName);
        }
    }
}