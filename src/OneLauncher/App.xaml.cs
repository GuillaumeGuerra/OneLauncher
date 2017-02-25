using System;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Windows;
using Autofac;
using Autofac.Extras.CommonServiceLocator;
using Microsoft.Practices.ServiceLocation;
using OneLauncher.Properties;
using OneLauncher.Services.MessageService;

namespace OneLauncher
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IContainer Container { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            ConfigureDependencyInjection();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        public static void ConfigureDependencyInjection()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyTypes(Assembly.GetExecutingAssembly()).PropertiesAutowired().AsImplementedInterfaces().AsSelf();

            Container = builder.Build();
            ServiceLocator.SetLocatorProvider(() => new AutofacServiceLocator(Container));
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            ServiceLocator.Current.GetInstance<IMessageService>().ShowException(e.ExceptionObject as Exception);
        }
    }
}
