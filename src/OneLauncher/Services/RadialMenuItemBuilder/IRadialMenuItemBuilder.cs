using System.Collections.Generic;
using Infragistics.Controls.Menus;
using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.Services.RadialMenuItemBuilder
{
    public interface IRadialMenuItemBuilder
    {
        IEnumerable<RadialMenuItem> BuildMenuItems(IEnumerable<LaunchersNode> launchers);
    }
}