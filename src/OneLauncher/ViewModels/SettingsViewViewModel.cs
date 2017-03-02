using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.AttributeExtensions;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Internal;
using OneLauncher.Framework;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.ViewModels
{
    [SingleInstance]
    public class SettingsViewViewModel : DependencyObject
    {
        public static readonly DependencyProperty RepositoriesProperty =
            DependencyProperty.Register("Repositories", typeof(TrulyObservableCollection<RepositoryViewModel>), typeof(SettingsViewViewModel));
        public static readonly DependencyProperty SettingsChangedProperty =
            DependencyProperty.Register("SettingsChanged", typeof(bool), typeof(SettingsViewViewModel), new PropertyMetadata(false));

        public bool SettingsChanged
        {
            get { return (bool)GetValue(SettingsChangedProperty); }
            set { SetValue(SettingsChangedProperty, value); }
        }

        public TrulyObservableCollection<RepositoryViewModel> Repositories
        {
            get { return (TrulyObservableCollection<RepositoryViewModel>)GetValue(RepositoriesProperty); }
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

        public ICommand ClosingCommand
        {
            get { return new RelayCommand<CancelEventArgs>(Closing); }
        }

        public SettingsViewViewModel()
        {
            App.Container.InjectProperties(this);

            Repositories = new TrulyObservableCollection<RepositoryViewModel>();
            Repositories.CollectionChanged += Repositories_CollectionChanged;
        }

        private void Repositories_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            SettingsChanged = true;
        }

        private void Loaded()
        {
            Repositories.Clear();

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

            SettingsChanged = false;
        }

        private void SaveSettings()
        {
            Context.UserSettings.Repositories = new Dictionary<string, List<Repository>>();

            foreach (var group in Repositories.GroupBy(r => r.Type))
            {
                Context.UserSettings.Repositories[group.Key] = group.Select(r => new Repository() { Name = r.Name, Path = r.Path }).ToList();
            }

            Context.SaveUserSettings();

            SettingsChanged = false;
        }

        private void Closing(CancelEventArgs e)
        {
            if (!SettingsChanged)
                return;

            if (MessageBox.Show(App.Current.Windows[0], "You did not save your changes, are you sure you quit ?", "Warning",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                e.Cancel = true;

        }
    }
}