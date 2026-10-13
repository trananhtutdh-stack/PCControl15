using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Machine;
using PCControl.Service.Config;
using PCControl.Service.Navigation;
using PCControl.Utility;
using ShareLibrary.Localizations;
using System.Windows.Controls;
using System.Windows.Input;

namespace PCControl.ViewModel
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly IAppNavigationService _navigation;
        [ObservableProperty]
        private object? currentView;
        public MachineDevices Devices { get; }

        private Config _config;

        public MachineStatus Status { get; }
        public MainWindowViewModel(IAppNavigationService navigation, MachineDevices devices, MachineStatus machineStatus, Config config)
        {
            Devices = devices;
            Status = machineStatus;
            _config = config;
            _navigation = navigation;
            _navigation.Navigated += view => CurrentView = view;
            _navigation.Navigate("Home");
            
        }

        [RelayCommand]
        private void Navigate(string? pageKey)
        {
            _navigation.Navigate(pageKey ?? string.Empty);
        }

        [RelayCommand]
        private void ChangeLanguage(string language)
        {
            LanguageManager.LoadLanguage(language);
        }
     

    }
}
