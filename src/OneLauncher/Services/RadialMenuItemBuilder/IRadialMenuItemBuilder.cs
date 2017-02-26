using System.Collections.Generic;
using Infragistics.Controls.Menus;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.ViewModels;

namespace OneLauncher.Services.RadialMenuItemBuilder
{
    public interface IRadialMenuItemBuilder
    {
        IEnumerable<RadialMenuItem> BuildMenuItems(IEnumerable<LaunchersNode> launchers, OneLauncherViewModel vm);
    }
}