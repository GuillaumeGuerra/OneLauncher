using System.IO;
using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.Services.CommandLauncher
{
    public class FileCopier : BaseCommandLauncher<FileCopierCommand>
    {
        protected override void DoExecute(FileCopierCommand command)
        {
            if (!File.Exists(command.SourceFilePath))
                throw new FileNotFoundException($"Unable to find file to copy [{command.SourceFilePath}]");

            if (!Directory.Exists(Path.GetDirectoryName(command.TargetFilePath)))
                throw new FileNotFoundException($"Unable to find target directory to copy to [{Path.GetDirectoryName(command.TargetFilePath)}]");

            File.Copy(command.SourceFilePath, command.TargetFilePath, true);
        }
    }
}