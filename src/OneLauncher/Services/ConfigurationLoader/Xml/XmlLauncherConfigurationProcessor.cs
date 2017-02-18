using System.Collections.Generic;
using System.IO;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherConfigurationProcessor : ILauncherConfigurationProcessor
    {
        public IXmlLauncherConfigurationReader ConfigurationReader { get; set; }

        public bool CanProcess(string path)
        {
            return Path.GetExtension(path) == ".xml";
        }

        public LaunchersNode Load(string path)
        {
            var launchers = new LaunchersNode();

            foreach (var root in ConfigurationReader.LoadFile(path).RootDirectories)
            {
                launchers.SubGroups.Add(ProcessNode(ConfigurationReader.LoadFile(path).GenericTemplate, root, root.Header));
            }

            return launchers;
        }

        private LaunchersNode ProcessNode(XmlLauncherNode node, XmlLauncherRootDirectory root, string header = null)
        {
            var result = new LaunchersNode() { Header = header ?? node.Header };

            if (node.SubGroups != null)
            {
                foreach (var group in node.SubGroups)
                {
                    result.SubGroups.Add(ProcessNode(group, root));
                }
            }

            if (node.Launchers != null)
            {
                foreach (var launcher in node.Launchers)
                {
                    result.Launchers.Add(ProcessLauncherLink(root, launcher));
                }
            }

            return result;
        }

        public LauncherLink ProcessLauncherLink(XmlLauncherRootDirectory root, XmlLauncherLink xmlLauncher)
        {
            var resolvedRootPath = root.Path;
            if (root.Path.EndsWith("/") || root.Path.EndsWith(@"\"))
                resolvedRootPath = resolvedRootPath.Substring(0, resolvedRootPath.Length - 1);

            var launcher = new LauncherLink()
            {
                Header = xmlLauncher.Header,
                Commands = new List<LauncherCommand>()
            };

            foreach (var command in xmlLauncher.Commands)
            {
                launcher.Commands.Add(command.ToCommand(resolvedRootPath));
            }

            return launcher;
        }
    }
}