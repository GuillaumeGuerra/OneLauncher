using System.Collections.Generic;
using System.IO;
using OneLauncher.Services.Context;
using OneLauncher.Services.MessageService;

namespace OneLauncher.Services.ConfigurationLoader.Xml
{
    public class XmlLauncherConfigurationProcessor : ILauncherConfigurationProcessor
    {
        public IXmlLauncherConfigurationReader ConfigurationReader { get; set; }

        public IOneLauncherContext Context { get; set; }

        public IMessageService MessageService { get; set; }

        public bool CanProcess(string path)
        {
            return Path.GetExtension(path) == ".xml";
        }

        public IEnumerable<LaunchersNode> Load(string path)
        {
            var configuration = ConfigurationReader.LoadFile(path);

            List<Repository> repos;
            if (!Context.UserSettings.Repositories.TryGetValue(configuration.RepoType, out repos))
            {
                MessageService.ShowErrorMessage($"Unable to find repo type {configuration.RepoType} in config file {Context.ApplicationSettings.UserSettingsDirectory}/{Context.ApplicationSettings.UserSettingsFileName}");
                yield break;
            }

            foreach (var root in Context.UserSettings.Repositories[configuration.RepoType])
            {
                yield return ProcessNode(configuration.GenericTemplate, root, root.Name);
            }
        }

        private LaunchersNode ProcessNode(XmlLauncherNode node, Repository root, string header = null)
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

        public LauncherLink ProcessLauncherLink(Repository root, XmlLauncherLink xmlLauncher)
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