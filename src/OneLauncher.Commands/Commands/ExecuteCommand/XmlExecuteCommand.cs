using System.Xml.Serialization;
using OneLauncher.Core.Commands;

namespace OneLauncher.Commands.Commands.ExecuteCommand
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