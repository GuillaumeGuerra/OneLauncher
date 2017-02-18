using System.Collections.Generic;

namespace OneLauncher.Services.ConfigurationLoader
{
    public class LaunchersNode
    {
        public List<LaunchersNode> SubGroups { get; set; }
        public List<LauncherLink> Launchers { get; set; }
        public string Header { get; set; }

        public LaunchersNode()
        {
            SubGroups = new List<LaunchersNode>();
            Launchers = new List<LauncherLink>();
        }
    }
}