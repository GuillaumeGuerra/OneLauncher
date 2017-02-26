using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.AttributeExtensions;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Internal;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.ViewModels
{
    [SingleInstance]
    public class SettingsViewViewModel : DependencyObject
    {
        public static readonly DependencyProperty RepositoriesProperty =
            DependencyProperty.Register("Repositories", typeof(ObservableCollection<RepositoryViewModel>), typeof(SettingsViewViewModel));

        public ObservableCollection<RepositoryViewModel> Repositories
        {
            get { return (ObservableCollection<RepositoryViewModel>)GetValue(RepositoriesProperty); }
            set { SetValue(RepositoriesProperty, value); }
        }

        public IOneLauncherContext Context { get; set; }

        public ICommand LoadedCommand
        {
            get { return new RelayCommand(Loaded); }
        }

        public ICommand SaveSettingsCommand
        {
            get { return new RelayCommand(SaveSettings); }
        }

        public SettingsViewViewModel()
        {
            App.Container.InjectProperties(this);
        }

        private async void Loaded()
        {
            Repositories = new ObservableCollection<RepositoryViewModel>();

            foreach (var repository in Context.UserSettings.Repositories)
            {
                foreach (var repo in repository.Value)
                {
                    Repositories.Add(new RepositoryViewModel()
                    {
                        Type = repository.Key,
                        Name = repo.Name,
                        Path = repo.Path
                    });
                }
            }
        }

        private void SaveSettings()
        {
            // TODO : write the settings into the context
            // TODO : save the context
        }
    }

    public class RepositoryViewModel
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}