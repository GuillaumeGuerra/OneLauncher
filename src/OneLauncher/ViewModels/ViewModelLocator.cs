using Microsoft.Practices.ServiceLocation;

namespace OneLauncher.ViewModels
{
    public class ViewModelLocator
    {
        public OmniLauncherViewModel MainViewModel => ServiceLocator.Current.GetInstance<OmniLauncherViewModel>();
    }
}