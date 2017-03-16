﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.AttributeExtensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Windows.Internal;
using OneLauncher.Core.Container;
using OneLauncher.Framework;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.ViewModels
{
    [SingleInstance]
    public class SettingsViewViewModel : ViewModelBase
    {
        private bool _settingsChanged;
        private TrulyObservableCollection<RepositoryViewModel> _repositories;
        private bool _aboutWindowVisibility;

        public IOneLauncherContext Context { get; set; }

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