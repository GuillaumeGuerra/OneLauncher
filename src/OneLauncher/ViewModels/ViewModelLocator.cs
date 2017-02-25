using Microsoft.Practices.ServiceLocation;

namespace OneLauncher.ViewModels
{
    public class ViewModelLocator
    {
        public OneLauncherViewModel MainViewModel => ServiceLocator.Current.GetInstance<OneLauncherViewModel>();
    }
}