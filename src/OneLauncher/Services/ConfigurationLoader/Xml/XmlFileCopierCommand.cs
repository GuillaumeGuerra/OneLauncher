using System.Xml.Serialization;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlName("CopyFile")]
    public class XmlFileCopierCommand : XmlLauncherCommand
    {
        [XmlAttribute]
        public string SourceFilePath { get; set; }

        [XmlAttribute]
        public string TargetFilePath { get; set; }

        public override LauncherCommand ToCommand(string resolvedRootPath)
        {
            return new FileCopierCommand()
            {
                SourceFilePath = SourceFilePath.Replace(RootToken, resolvedRootPath),
                TargetFilePath = TargetFilePath.Replace(RootToken, resolvedRootPath)
            };
        }
    }
}