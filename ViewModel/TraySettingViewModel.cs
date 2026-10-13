using PCControl.Service.DeviceManager.Actuator;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using PCControl.Service.Config;

namespace PCControl.ViewModel
{
    public partial class TraySettingViewModel : ObservableObject
    {
        public TRAY Tray { get; }
        public PALLET PalletTemp { get; }
        private Config _config;

        public TraySettingViewModel(TRAY tray, Config config)
        {
            _config = config;
            this.Tray = tray;
            PalletTemp = this.Tray.Pallet.Coppy();
        }

        [RelayCommand]
        private void Save(Window window)
        {
            Tray.Pallet.Row = PalletTemp.Row;
            Tray.Pallet.Col = PalletTemp.Col;
            Tray.Pallet.PitchX = PalletTemp.PitchX;
            Tray.Pallet.PitchY = PalletTemp.PitchY;
            _config.SaveRecipeSettingJson();
            window.Close();
        }

        [RelayCommand]
        private void Cancel(Window window)
        {            
            window.Close();
        }
    }
}
