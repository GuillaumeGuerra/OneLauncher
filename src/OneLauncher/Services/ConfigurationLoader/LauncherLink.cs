using System.Collections.Generic;
using OneLauncher.Core.Commands;

namespace OneLauncher.Services.ConfigurationLoader
{
    public class LauncherLink
    {
        public string Header { get; set; }
        public List<LauncherCommand> Commands { get; set; }
    }
}