using System.Diagnostics;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.ExecuteCommand
{
    public class ExecuteCommandLauncher : BaseCommandLauncher<ExecuteCommand>
    {
        protected override void DoExecute(ExecuteCommand command)
        {
            Process.Start(command.Command);
        }
    }
}