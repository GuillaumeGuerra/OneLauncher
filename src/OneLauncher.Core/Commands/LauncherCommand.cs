using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using OneLauncher.Core.Container;

namespace OneLauncher.Core.Commands
{
    [KnownType("GetKnownCommands")]
    public class LauncherCommand
    {
        public static IEnumerable<Type> GetKnownCommands()
        {
            return OneLauncherContainer.Instance.GetRegisteredSubClasses<LauncherCommand>();
        }
    }
}