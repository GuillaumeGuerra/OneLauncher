﻿using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Controls.Menus;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.RadialMenuItemBuilder;

namespace OneLauncher.ViewModels
{
    public class OneLauncherViewModel : DependencyObject
    {
        public static readonly DependencyProperty LaunchersProperty =
            DependencyProperty.Register("Launchers", typeof(ObservableCollection<RadialMenuItem>), typeof(OneLauncherViewModel), new PropertyMetadata(new ObservableCollection<RadialMenuItem>()));
        public static readonly DependencyProperty IsOpenedProperty =
            DependencyProperty.Register("IsOpened", typeof(bool), typeof(OneLauncherViewModel), new PropertyMetadata(false));

        public ObservableCollection<RadialMenuItem> Launchers
        {
            get { return (ObservableCollection<RadialMenuItem>)GetValue(LaunchersProperty); }
            set { SetValue(LaunchersProperty, value); }
        }

        public bool IsOpened
        {
            get { return (bool)GetValue(IsOpenedProperty); }
            set { SetValue(IsOpenedProperty, value); }
        }

        public IRadialMenuItemBuilder RadialMenuItemBuilder { get; set; }

        public IConfigurationLoader ConfigurationLoader { get; set; }

        public ICommand ClosedCommand
        {
            get { return new RelayCommand(Closed); }
        }

        public ICommand LoadedCommand
        {
            get { return new RelayCommand(Loaded); }
        }

        public OneLauncherViewModel()
        {
            App.Container.InjectProperties(this);
        }

        private async void Loaded()
        {
            var launchersNodes = await Task.Run(() => ConfigurationLoader.LoadConfiguration("Launchers"));

            Launchers = new ObservableCollection<RadialMenuItem>(RadialMenuItemBuilder.BuildMenuItems(launchersNodes));

            // Now that the radial menu is ready, we can make it visible
            IsOpened = true;
        }

        private void Closed()
        {
            Application.Current.Shutdown();
        }
    }
}
