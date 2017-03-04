using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Infragistics.Controls.Menus;
using OneLauncher.Framework;
using OneLauncher.Properties;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.LauncherService;
using OneLauncher.ViewModels;

namespace OneLauncher.Services.RadialMenuItemBuilder
{
    public class RadialMenuItemBuilder : IRadialMenuItemBuilder
    {
        public ILauncherService LauncherService { get; set; }

        public IEnumerable<RadialMenuItem> BuildMenuItems(IEnumerable<LaunchersNode> launchers, OneLauncherViewModel vm)
        {
            var items = new List<RadialMenuItem>();

            if (launchers != null)
            {
                foreach (var item in launchers)
                {
                    items.Add(GetNodeMenuItem(item));
                }
            }

            var settingsButton = GetSettingsButton(vm);

            items.Add(settingsButton);

            return items;
        }

        public RadialMenuItem GetNodeMenuItem(LaunchersNode launchers)
        {
            var item = new RadialMenuItem { Header = launchers.Header };

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

        private RadialMenuItem GetSettingsButton(OneLauncherViewModel vm)
        {
            var settingsButton = new RadialMenuItem
            {
                Header = "Settings",
                Icon = new Image()
                {
                    Source = Resources.Settings.ToImageSource(),
                    Width = 25,
                    Height = 25
                }
            };

            settingsButton.Click += (s, e) => vm.OpenSettingsCommand.Execute(null);

            return settingsButton;
        }
    }
}