using GalaSoft.MvvmLight;
using OneLauncher.Services.ConfigurationLoader;

namespace OneLauncher.ViewModels
{
    public class LauncherViewModel : ViewModelBase
    {
        private DiscoveredLauncher _innerDiscoveredLauncher;
        private bool _active;

        public DiscoveredLauncher InnerDiscoveredLauncher
        {
            get { return _innerDiscoveredLauncher; }
            set
            {
                _innerDiscoveredLauncher = value;
                RaisePropertyChanged(null);
            }
        }

        public bool Active
        {
            get { return _active; }
            set
            {
                _active = value;
                RaisePropertyChanged();
            }
        }
        public string Name { get { return System.IO.Path.GetFileName(InnerDiscoveredLauncher.FilePath); } }
        public string Path { get { return InnerDiscoveredLauncher.FilePath; } }
        public string Loader { get { return InnerDiscoveredLauncher.Processor.GetType().Name; } }
    }
}