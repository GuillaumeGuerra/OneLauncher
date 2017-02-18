using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OneLauncher.Framework;

namespace OneLauncher.Services.ConfigurationLoader
{
    public class ConfigurationLoader : IConfigurationLoader
    {
        public IEnumerable<ILauncherConfigurationProcessor> AllConfigurationProcessors { get; set; }

        public IEnumerable<LaunchersNode> LoadConfiguration(string path)
        {
            var launchers = new List<LaunchersNode>();

            foreach (var file in Directory.GetFiles(path, "*.*", SearchOption.AllDirectories))
            {
                var plugin = AllConfigurationProcessors.FirstOrDefault(p => p.CanProcess(file));

                if (plugin != null)
                    launchers.Add(plugin.Load(file));
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