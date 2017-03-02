using GalaSoft.MvvmLight;

namespace OneLauncher.ViewModels
{
    public class RepositoryViewModel : ViewModelBase
    {
        private string _type;
        private string _name;
        private string _path;

        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                RaisePropertyChanged();
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }

        public string Path
        {
            get { return _path; }
            set
            {
                _path = value;
                RaisePropertyChanged();
            }
        }
    }
}