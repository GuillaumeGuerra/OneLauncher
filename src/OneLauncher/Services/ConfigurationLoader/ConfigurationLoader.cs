using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneLauncher.Framework;
using OneLauncher.Services.Context;

namespace OneLauncher.Services.ConfigurationLoader
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        public IEnumerable<ILauncherConfigurationProcessor> AllConfigurationProcessors { get; set; }
        public IOneLauncherContext Context { get; set; }

        public IEnumerable<LaunchersNode> LoadConfiguration(string path)
        {
            var launchers = Enumerable.Empty<LaunchersNode>();

            var binaryLaunchers = Directory.GetFiles(Path.Combine(path, "Launchers"), "*.*", SearchOption.AllDirectories);
            var userLaunchersDirectory = Path.Combine(Context.ApplicationSettings.UserSettingsDirectory, "Launchers");
            var userLaunchers = Directory.Exists(userLaunchersDirectory) ? Directory.GetFiles(userLaunchersDirectory, "*.*", SearchOption.AllDirectories) : new string[0];

            foreach (var file in binaryLaunchers.Concat(userLaunchers))
            {
                var plugin = AllConfigurationProcessors.FirstOrDefault(p => p.CanProcess(file));

                if (plugin != null)
                    launchers = launchers.Concat(plugin.Load(file));
            }

            return MergeLaunchers(launchers);
        }

        private IEnumerable<LaunchersNode> MergeLaunchers(IEnumerable<LaunchersNode> launchers)
        {
            var groupedLaunchers = from l in launchers
                                   group l by l.Header;

            foreach (var group in groupedLaunchers)
            {
                var merge = group.First();
                foreach (var toMerge in group.Skip(1))
                {
                    merge = MergeTwoLaunchers(merge, toMerge);
                }

                yield return merge;
            }
        }

        private LaunchersNode MergeTwoLaunchers(LaunchersNode left, LaunchersNode right)
        {
            var result = left.DeepClone();

            result.Launchers = new List<LauncherLink>(left.Launchers); // To keep the same reference (it eases the UTs a lot, trust me ...)
            result.Launchers.AddRange(right.Launchers);
            result.SubGroups = MergeLaunchers(left.SubGroups.Concat(right.SubGroups)).ToList();

            return result;
        }
    }
}