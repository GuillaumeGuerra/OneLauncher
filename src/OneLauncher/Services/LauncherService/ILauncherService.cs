using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.Services.LauncherService
{
    public interface ILauncherService
    {
        void Launch(LauncherLink launcher);
    }
}