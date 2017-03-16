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
        public Dictionary<string, List<Repository>> Repositories { get; set; }

        [JsonProperty]
        public List<string> ExcludedLauncherFilePaths { get; set; }

        [JsonIgnore]
        public bool IsDefaultSettings { get; set; }

        public UserSettings()
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            SettingsVersion = $"{version.Major}.{version.Minor}";
            Repositories = new Dictionary<string, List<Repository>>();
        }

        public static UserSettings GetDefaultSettings()
        {
            return new UserSettings()
            {
                Repositories = new Dictionary<string, List<Repository>>()
                {
                    {"XONE",new List<Repository>()
                    {
                        new Repository(){Name = "MASTER",Path = @"D:/DEV/XOneMaster"},
                        new Repository(){Name = "PROD",Path = @"D:/DEV/XOneProd"}
                    } }
                },
                IsDefaultSettings = true
            };
        }
    }
}
