using System.Xml.Serialization;
using OneLauncher.Services.CommandLauncher;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    [XmlName("Execute")]
    public class XmlExecuteCommand : XmlLauncherCommand
    {
        [XmlAttribute]
        public string Command { get; set; }

        public override LauncherCommand ToCommand(string resolvedRootPath)
        {
            return new ExecuteCommand() { Command = Command.Replace(RootToken, resolvedRootPath) };
        }
    }
}