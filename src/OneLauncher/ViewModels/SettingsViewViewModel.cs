using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.AttributeExtensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Internal;
using OneLauncher.Core.Container;
using OneLauncher.Framework;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.ViewModels
{
    [SingleInstance]
    public class SettingsViewViewModel : ViewModelBase
    {
        private bool _settingsChanged;
        private TrulyObservableCollection<RepositoryViewModel> _repositories = new TrulyObservableCollection<RepositoryViewModel>();
        private TrulyObservableCollection<LauncherViewModel> _launchers = new TrulyObservableCollection<LauncherViewModel>();
        private bool _aboutWindowVisibility;

        public IOneLauncherContext Context { get; set; }
        public IConfigurationLoader Loader { get; set; }

        public bool SettingsChanged
        {
            get { return _settingsChanged; }
            set
            {
                _settingsChanged = value;
                RaisePropertyChanged();
            }
        }
        public TrulyObservableCollection<RepositoryViewModel> Repositories
        {
            get { return _repositories; }
            set
            {
                _repositories = value;
                RaisePropertyChanged();
            }
        }
        public TrulyObservableCollection<LauncherViewModel> Launchers
        {
            get { return _launchers; }
            set
            {
                _launchers = value;
                RaisePropertyChanged();
            }
        }
        public bool AboutWindowVisibility
        {
            get { return _aboutWindowVisibility; }
            set
            {
                _aboutWindowVisibility = value;
                RaisePropertyChanged();
            }
        }

        public ICommand LoadedCommand
        {
            get { return new RelayCommand(Loaded); }
        }
        public ICommand SaveSettingsCommand
        {
            get { return new RelayCommand(SaveSettings); }
        }
        public ICommand ClosingCommand
        {
            get { return new RelayCommand<CancelEventArgs>(Closing); }
        }
        public ICommand OpenAboutWindowCommand
        {
            get { return new RelayCommand(OpenWindow); }
        }
        public ICommand NavigateToUriCommand
        {
            get { return new RelayCommand<string>(Navigate); }
        }

        public SettingsViewViewModel()
        {
            OneLauncherContainer.Instance.InjectProperties(this);

            Repositories.CollectionChanged += Repositories_CollectionChanged;
            Launchers.CollectionChanged += Repositories_CollectionChanged;
        }

        private void Repositories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsChanged = true;
        }

        private async void Loaded()
        {
            List<RepositoryViewModel> tmpRepositories = null;
            List<LauncherViewModel> tmpLaunchers = null;
            await Task.Run(() =>
            {
                tmpRepositories = GetRepositories();
                tmpLaunchers = GetLaunchers();
            });

            Repositories.Clear();
            Repositories.AddRange(tmpRepositories);

            Launchers.Clear();
            Launchers.AddRange(tmpLaunchers);

            SettingsChanged = false;
        }

        private List<LauncherViewModel> GetLaunchers()
        {
            var launchers = new List<LauncherViewModel>();
            var excludedLaunchers = new HashSet<string>(Context.UserSettings.ExcludedLauncherFilePaths ?? new List<string>());

            foreach (var launcher in Loader.DiscoverFiles(Environment.CurrentDirectory))
            {
                launchers.Add(new LauncherViewModel() { InnerDiscoveredLauncher = launcher, Active = !excludedLaunchers.Contains(launcher.FilePath) });
            }
            return launchers;
        }

        private List<RepositoryViewModel> GetRepositories()
        {
            var repositories = new List<RepositoryViewModel>();

            foreach (var repository in Context.UserSettings.Repositories)
            {
                foreach (var repo in repository.Value)
                {
                    repositories.Add(new RepositoryViewModel()
                    {
                        Type = repository.Key,
                        Name = repo.Name,
                        Path = repo.Path
                    });
                }
            }

            return repositories;
        }

        private void SaveSettings()
        {
            Context.UserSettings.Repositories = new Dictionary<string, List<Repository>>();

            foreach (var group in Repositories.GroupBy(r => r.Type))
            {
                Context.UserSettings.Repositories[group.Key] = group.Select(r => new Repository() { Name = r.Name, Path = r.Path }).ToList();
            }

            Context.UserSettings.ExcludedLauncherFilePaths =
                Launchers.Where(l => !l.Active).Select(l => l.Path).ToList();

            Context.SaveUserSettings();

            SettingsChanged = false;
        }

        private void Closing(CancelEventArgs e)
        {
            if (!SettingsChanged)
                return;

            if (MessageBox.Show(App.Current.Windows[0], "You did not save your changes, are you sure you want to quit ?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;

        }

        private void OpenWindow()
        {
            AboutWindowVisibility = true;
        }

        private void Navigate(string url)
        {
            Process.Start(url);
        }
    }
}