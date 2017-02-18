namespace OneLauncher.Services.ConfigurationLoader
{
    public class FileCopierCommand : LauncherCommand
    {
        public string SourceFilePath { get; set; }

        public string TargetFilePath { get; set; }
    }
}