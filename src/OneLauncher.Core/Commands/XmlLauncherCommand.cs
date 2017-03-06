using System.Xml.Serialization;

namespace OneLauncher.Core.Commands
{
    public abstract class XmlLauncherCommand
    {
        protected const string RootToken = "[ROOT]";

        public abstract LauncherCommand ToCommand(string resolvedRootPath);
    }
}