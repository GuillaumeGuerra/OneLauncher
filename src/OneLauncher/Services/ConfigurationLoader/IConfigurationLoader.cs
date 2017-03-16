using System.Collections.Generic;

namespace OneLauncher.Services.ConfigurationLoader
{
    public interface IConfigurationLoader
    {
        IEnumerable<LaunchersNode> LoadConfiguration(string path);

        IEnumerable<DiscoveredLauncher> DiscoverFiles(string path);
    }
}