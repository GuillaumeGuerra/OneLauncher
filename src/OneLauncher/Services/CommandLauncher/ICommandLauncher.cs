using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.Services.CommandLauncher
{
    public interface ICommandLauncher
    {
        bool CanProcess(LauncherCommand command);
        void Execute(LauncherCommand command);
    }
}
