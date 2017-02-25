using System;
using System.Configuration;
using System.IO;
using Newtonsoft.Json;

namespace OneLauncher.Services.Context
{
    public interface IOneLauncherContext
    {
        ApplicationSettings ApplicationSettings { get; }

        UserSettings UserSettings { get; }

        void SaveUserSettings();
    }

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
            File.WriteAllText(Path.Combine(ApplicationSettings.UserSettingsDirectory, ApplicationSettings.UserSettingsFileName), json);
        }

        private UserSettings LoadUserSettings()
        {
            var path = Path.Combine(ApplicationSettings.UserSettingsDirectory, ApplicationSettings.UserSettingsFileName);
            if (File.Exists(path))
                return JsonConvert.DeserializeObject<UserSettings>(File.ReadAllText(path));

            return new UserSettings();
        }
    }
}