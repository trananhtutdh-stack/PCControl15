using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Machine;

namespace PCControl.ViewModel
{
    public partial class ManualViewModel : ObservableObject
    {
        public MachineDevices Devices { get; }

        public ManualViewModel(MachineDevices devices)
        {
            Devices = devices;
        }

        [RelayCommand]
        private void MoveCh1Jig()
        {
            Devices?.CYL_JIG_CH1?.Move();
        }

        [RelayCommand]
        private void OriginCh1Jig()
        {
            Devices?.CYL_JIG_CH1?.Origin();
        }

        [RelayCommand]
        private void MoveCh2Jig()
        {
            Devices?.CYL_JIG_CH2?.Move();
        }

        [RelayCommand]
        private void OriginCh2Jig()
        {
            Devices?.CYL_JIG_CH2?.Origin();
        }

        [RelayCommand]
        private void MoveTray()
        {
            Devices?.CYL_TRAY?.Move();
        }

        [RelayCommand]
        private void OriginTray()
        {
            Devices?.CYL_TRAY?.Origin();
        }
    }
}
