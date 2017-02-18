using System.Collections.Generic;

namespace OneLauncher.Services.ConfigurationLoader
{
    public class LauncherLink
    {
        public string Header { get; set; }
        public List<LauncherCommand> Commands { get; set; }
    }
}