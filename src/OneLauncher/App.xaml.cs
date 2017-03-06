using System;
using System.Windows;
using Autofac;
using Microsoft.Practices.ServiceLocation;
using OneLauncher.Core.Container;
using OneLauncher.Services.MessageService;

namespace OneLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            OneLauncherContainer.ConfigureDependencyInjection();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            OneLauncherContainer.Instance.Resolve<IMessageService>().ShowException(e.ExceptionObject as Exception);
        }
    }
}
