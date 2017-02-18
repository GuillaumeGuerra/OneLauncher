using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace OneLauncher.Services.ConfigurationLoader
{
    [KnownType("GetKnownCommands")]
    public class LauncherCommand
    {
        public static IEnumerable<Type> GetKnownCommands()
        {
            return App.Container.ComponentRegistry.Registrations
                .Where(r => typeof(LauncherCommand).IsAssignableFrom(r.Activator.LimitType))
                .Select(r => r.Activator.LimitType);
        }
    }
}