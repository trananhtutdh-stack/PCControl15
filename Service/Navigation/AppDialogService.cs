using AppConfig = PCControl.Service.Config.Config;
using PCControl.Service.Config;
using PCControl.Service.DeviceManager.Actuator;
using PCControl.ViewModel;
using PCControl.Views.Dialog;

namespace PCControl.Service.Navigation
{
    public class AppDialogService : IActuatorDialogService
    {
        private readonly AppConfig _config;

        public AppDialogService(AppConfig config)
        {
            _config = config;
        }

        public object? ShowDialog(AppDialogIds.DIALOG_ID dialog, object obj)
        {
            switch (dialog)
            {
                default:
                    return null;
            }
        }

        public bool? ShowJigSetting(JIG jig)
        {
            var window = new JigSetting
            {
                DataContext = new JigSettingViewModel(jig, _config)
            };
            return window.ShowDialog();
        }

        public bool? ShowTraySetting(TRAY tray)
        {
            var window = new TraySetting
            {
                DataContext = new TraySettingViewModel(tray, _config)
            };
            return window.ShowDialog();
        }
    }
}
