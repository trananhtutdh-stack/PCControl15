namespace PCControl.Service
{
    public interface IExternal : IAlarmReset
    {
        bool AirSignal { get; set; }

        bool ManualAutoMode { get; set; }

        bool InitializingDone { get; set; }
    }

    public interface IAlarmReset
    {
        bool AlarmReset { get; set; }
    }
}
