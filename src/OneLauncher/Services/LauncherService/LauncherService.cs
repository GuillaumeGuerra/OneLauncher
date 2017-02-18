using System;
using System.Collections.Generic;
using System.Linq;
using OneLauncher.Services.CommandLauncher;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.MessageService;

namespace OneLauncher.Services.LauncherService
{
    public class LauncherService : ILauncherService
    {
        public IMessageService MessageService { get; set; }

        public IEnumerable<ICommandLauncher> AllCommandLaunchers { get; set; }

        public void Launch(LauncherLink launcher)
        {
            try
            {
                foreach (var command in launcher.Commands)
                {
                    var plugin = AllCommandLaunchers.FirstOrDefault(p => p.CanProcess(command));
                    if (plugin == null)
                        throw new NotSupportedException($"Unable to find a CommandLauncher plugin for the type {command.GetType().FullName}");

                    plugin.Execute(command);
                }
            }
            catch (Exception exception)
            {
                MessageService.ShowException(exception);
            }
        }
    }
}