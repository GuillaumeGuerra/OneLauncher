using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Autofac.AttributeExtensions;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Infragistics.Controls.Menus;
using OneLauncher.Core.Container;
using OneLauncher.Services.ConfigurationLoader;
using OneLauncher.Services.Context;
using OneLauncher.Services.RadialMenuItemBuilder;
using OneLauncher.Views;

namespace OneLauncher.ViewModels
{
    [SingleInstance]
    public class OneLauncherViewModel : ViewModelBase
    {
        private ObservableCollection<RadialMenuItem> _launchers = new ObservableCollection<RadialMenuItem>();
        private bool _isOpened;

        public ObservableCollection<RadialMenuItem> Launchers
        {
            get { return _launchers; }
            set
            {
                _launchers = value;
                RaisePropertyChanged();
            }
        }
        public bool IsOpened
        {
            get { return _isOpened; }
            set
            {
                _isOpened = value;
                RaisePropertyChanged();
            }
        }

        public IRadialMenuItemBuilder RadialMenuItemBuilder { get; set; }
        public IConfigurationLoader ConfigurationLoader { get; set; }
        public IOneLauncherContext Context { get; set; }

        public ICommand ClosedCommand
        {
            get { return new RelayCommand(Closed); }
        }
        public ICommand LoadedCommand
        {
            get { return new RelayCommand(Loaded); }
        }
        public ICommand OpenSettingsCommand
        {
            get { return new RelayCommand(OpenSettings); }
        }

        public OneLauncherViewModel()
        {
            OneLauncherContainer.Instance.InjectProperties(this);
        }

        private async void Loaded()
        {
            if (Context.UserSettings.IsDefaultSettings)
                OpenSettings();

            await FillLaunchers();
        }

        private async Task FillLaunchers()
        {
            var launchersNodes = await Task.Run(() => ConfigurationLoader.LoadConfiguration(Environment.CurrentDirectory));

            Launchers = new ObservableCollection<RadialMenuItem>(RadialMenuItemBuilder.BuildMenuItems(launchersNodes, this));

            // Now that the radial menu is ready, we can make it visible
            IsOpened = true;
        }

        private void Closed()
        {
            Application.Current.Shutdown();
        }

        private async void OpenSettings()
        {
            OneLauncherContainer.Instance.Resolve<ISettingsView>().ShowDialog();

            await FillLaunchers();
        }
    }
}
