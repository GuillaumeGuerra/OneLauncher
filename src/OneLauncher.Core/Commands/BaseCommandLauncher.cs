using System;

namespace OneLauncher.Core.Commands
{
    public abstract class BaseCommandLauncher<TCommand> : ICommandLauncher
        where TCommand : LauncherCommand
    {
        public bool CanProcess(LauncherCommand command)
        {
            return command is TCommand;
        }

        public void Execute(LauncherCommand command)
        {
            if (!(command is TCommand))
                throw new NotSupportedException($"Invalid command type [{command.GetType().FullName}] given to CommandLauncher plugin [{GetType().FullName}]");

            DoExecute((TCommand)command);
        }

        protected abstract void DoExecute(TCommand command);
    }
}