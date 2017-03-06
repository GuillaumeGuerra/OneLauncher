using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Autofac;
using Autofac.AttributeExtensions;

namespace OneLauncher.Core.Container
{
    public class OneLauncherContainer
    {
        public static IContainer Instance { get; private set; }

        public static void ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();

            // Using Autofac, we will load all assemblies from the current directory that are called OneLauncherXXXX.dll or OneLauncherXXXX.exe
            // This way, all commands will be registered in the dependency injection system, to read xml files and execute commands

            var isOneLauncherAssemblyRegex = new Regex(@"^OneLauncher[\.\w]*\b(\.exe|\.dll)\b$", RegexOptions.Compiled);
            foreach (var path in Directory.EnumerateFiles(Environment.CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                .Where(s => isOneLauncherAssemblyRegex.IsMatch(Path.GetFileName(s))))
             {
                //Specific case for the UTs : do not load the UT assembly
                 if (Path.GetFileName(path).Equals("OneLauncher.Tests.dll", StringComparison.InvariantCultureIgnoreCase))
                     continue;

                var assembly = Assembly.LoadFrom(path);
                builder.RegisterAssemblyTypes(assembly).PropertiesAutowired().AsImplementedInterfaces().AsSelf();
                builder.RegisterAttributedClasses(assembly);
            }

            Instance = builder.Build();
        }
    }
}
