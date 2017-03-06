using Autofac;
using Microsoft.Practices.ServiceLocation;
using OneLauncher.Core.Container;

namespace OneLauncher.ViewModels
{
    public class ViewModelLocator
    {
        public OneLauncherViewModel MainViewModel => OneLauncherContainer.Instance.Resolve<OneLauncherViewModel>();

        public SettingsViewViewModel SettingsViewViewModel => OneLauncherContainer.Instance.Resolve<SettingsViewViewModel>();
    }
}