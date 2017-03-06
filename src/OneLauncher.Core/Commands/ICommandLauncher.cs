namespace OneLauncher.Core.Commands
{
    public interface ICommandLauncher
    {
        bool CanProcess(LauncherCommand command);
        void Execute(LauncherCommand command);
    }
}
