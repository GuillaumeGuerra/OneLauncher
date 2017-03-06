using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.CopyFile
{
    public class FileCopierCommand : LauncherCommand
    {
        public string SourceFilePath { get; set; }

        public string TargetFilePath { get; set; }
    }
}