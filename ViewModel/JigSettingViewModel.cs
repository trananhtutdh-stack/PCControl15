using PCControl.Service.DeviceManager.Actuator;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using PCControl.Service.Config;

namespace PCControl.ViewModel
{
    public partial class JigSettingViewModel : ObservableObject
    {
        public JIG Jig { get; }
        public PALLET PalletTemp { get; }
        private Config _config;

        public JigSettingViewModel(JIG jig, Config config)
        {
            _config = config;
            Jig = jig;
            PalletTemp = this.Jig.Pallet.Coppy();
        }

        [RelayCommand]
        private void SelectJig(JigItem item)
        {
            item.Status = !item.Status;
        }

        [RelayCommand]
        private void Save(Window window)
        {
            Jig.Pallet.Row = PalletTemp.Row;
            Jig.Pallet.Col = PalletTemp.Col;
            Jig.Pallet.PitchX = PalletTemp.PitchX;
            Jig.Pallet.PitchY = PalletTemp.PitchY;
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
