using Autofac;
using Microsoft.Practices.ServiceLocation;

namespace OneLauncher.ViewModels
{
    public class ViewModelLocator
    {
        public OneLauncherViewModel MainViewModel => App.Container.Resolve<OneLauncherViewModel>();

        public SettingsViewViewModel SettingsViewViewModel => App.Container.Resolve<SettingsViewViewModel>();
    }
}