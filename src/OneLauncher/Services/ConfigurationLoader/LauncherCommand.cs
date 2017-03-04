using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using OneLauncher.Framework;

namespace OneLauncher.Services.ConfigurationLoader
{
    [KnownType("GetKnownCommands")]
    public class LauncherCommand
    {
        public static IEnumerable<Type> GetKnownCommands()
        {
            return App.Container.GetRegisteredSubClasses<LauncherCommand>();
        }
    }
}