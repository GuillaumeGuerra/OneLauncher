using System.Diagnostics;

namespace OneLauncher.Services.CommandLauncher
{
    public class ExecuteCommandLauncher : BaseCommandLauncher<ExecuteCommand>
    {
        protected override void DoExecute(ExecuteCommand command)
        {
            Process.Start(command.Command);
        }
    }
}