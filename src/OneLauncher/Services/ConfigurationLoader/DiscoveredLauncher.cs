namespace OneLauncher.Services.ConfigurationLoader
{
    public class DiscoveredLauncher
    {
        public string FilePath { get; set; }
        public ILauncherConfigurationProcessor Processor { get; set; }
    }
}