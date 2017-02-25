using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;

namespace OneLauncher.Services.Context
{
    public class UserSettings
    {
        [JsonProperty]
        public string SettingsVersion { get; set; }

        [JsonProperty]
        public Dictionary<string, List<string>> Repositories { get; set; }

        public UserSettings()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            SettingsVersion = $"{version.Major}.{version.Minor}";
            Repositories = new Dictionary<string, List<string>>();
        }

    }

    public interface IUserSettingsService
    {
        UserSettings GetUserSettings();

        void SaveUserSettings(UserSettings settings);
    }

    public interface IApplicationSettingsService
    {
        ApplicationSettings GetApplicationSettings();
    }

    public class ApplicationSettings
    {
        public string UserSettingsDirectory { get; set; }

        public string UserSettingsFileName { get; set; }
    }
}
