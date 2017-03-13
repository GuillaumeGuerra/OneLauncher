using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework.Internal;
using NUnit.Framework;
using OneLauncher.Core.Commands;
using OneLauncher.Core.Container;

namespace OneLauncher.Commands.Tests.Commands
{
    [TestFixture]
    public class CommandsTests
    {
        [Test]
        public void AllCommandLauncherPluginsShouldHaveADedicatedTestClass()
        {
            var testedTypes =
                Assembly.GetExecutingAssembly()
                    .GetTypes()
                    .Where(
                        t =>
                            t.BaseType != null &&
                            t.BaseType.IsGenericType &&
                            t.BaseType.GetGenericTypeDefinition() == typeof(CommonCommandLauncherTests<,>))
                            .Select(t => t.BaseType.GetGenericArguments()[0])
                    .ToList();

            foreach (var plugin in OneLauncherContainer.Instance.GetImplementations<ICommandLauncher>())
            {
                var pluginType = plugin.GetType();
                if (testedTypes.All(t => t != pluginType))
                    Assert.Fail($"Missing test class for ICommandLauncher type [{pluginType.FullName}]");
            }
        }
    }
}
