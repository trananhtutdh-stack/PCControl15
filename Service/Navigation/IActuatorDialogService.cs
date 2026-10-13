using PCControl.Service.DeviceManager.Actuator;

namespace PCControl.Service.Navigation
{
    public interface IActuatorDialogService : IAppDialogService
    {
        bool? ShowJigSetting(JIG jig);

        bool? ShowTraySetting(TRAY tray);
    }
    public interface IAppDialogService
    {
        object? ShowDialog(AppDialogIds.DIALOG_ID dialog, object obj);
    }

    public static class AppDialogIds
    {
        public enum DIALOG_ID
        {
            DL_COM_SCANNER_ST = 0,
            DL_TCP_SCANNER_ST
        }
    }
}
