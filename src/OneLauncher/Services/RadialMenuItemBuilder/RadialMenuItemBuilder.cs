using System.Collections.Generic;
using Infragistics.Controls.Menus;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.LauncherService;

namespace OneLauncher.Services.RadialMenuItemBuilder
{
    public class RadialMenuItemBuilder : IRadialMenuItemBuilder
    {
        public ILauncherService LauncherService { get; set; }

        public IEnumerable<RadialMenuItem> BuildMenuItems(LaunchersNode launchers)
        {
            var items = new List<RadialMenuItem>();

            if (launchers != null)
            {
                foreach (var item in launchers.SubGroups)
                {
                    items.Add(GetNodeMenuItem(item));
                }
            }

            return items;
        }

        public RadialMenuItem GetNodeMenuItem(LaunchersNode launchers)
        {
            var item = new RadialMenuItem {Header = launchers.Header};

            foreach (var group in launchers.SubGroups)
            {
                item.Items.Add(GetNodeMenuItem(group));
            }
            foreach (var launcher in launchers.Launchers)
            {
                item.Items.Add(GetLauncherMenuItem(launcher));
            }

            return item;
        }

        private RadialMenuItem GetLauncherMenuItem(LauncherLink launcher)
        {
            var button = new RadialMenuItem() { Header = launcher.Header };

            button.Click += (s, e) => LauncherService.Launch(launcher);

            return button;
        }
    }
}