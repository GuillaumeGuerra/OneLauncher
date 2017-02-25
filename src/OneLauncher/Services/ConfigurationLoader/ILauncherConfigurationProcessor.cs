using System.Collections.Generic;

namespace OneLauncher.Services.ConfigurationLoader
{
    public interface ILauncherConfigurationProcessor
    {
        bool CanProcess(string path);

        IEnumerable<LaunchersNode> Load(string path);
    }
}