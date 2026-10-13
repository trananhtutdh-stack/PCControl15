using PCControl.Service.IPC;
using PCControl.Service.Config;
using AutoVision.Service.IPC;

namespace PCControl.Service
{
    public abstract class Cylinder : Automatic
    {
        private static readonly HashSet<CylinderSt> WiredSettings = [];

        public CylinderSt Settings { get; }

        public CylinderCondition Condition { get; set; } = null!;

        protected Cylinder(CylinderSt settings, string name)
        {
            if (!WiredSettings.Add(settings))
                throw new InvalidOperationException(
                    $"Cylinder '{name}' reuses CylinderSt already wired to another cylinder.");

            Settings = settings;
            EnsureCondition();
            Condition.NoMoveShield = Settings.NoMoveShield;
            Condition.NoOriginShield = Settings.NoOriginShield;
        }

        public int OriginTime
        {
            get => Settings.OriginTime;
            set
            {
                if (Settings.OriginTime == value)
                    return;

                Settings.OriginTime = value;
                OnPropertyChanged();
            }
        }

        public int MoveTime
        {
            get => Settings.MoveTime;
            set
            {
                if (Settings.MoveTime == value)
                    return;

                Settings.MoveTime = value;
                OnPropertyChanged();
            }
        }

        public int AlarmTime
        {
            get => Settings.AlarmTime;
            set
            {
                if (Settings.AlarmTime == value)
                    return;

                Settings.AlarmTime = value;
                OnPropertyChanged();
            }
        }

        public bool NoMoveShield
        {
            get => Settings.NoMoveShield;
            set
            {
                if (Settings.NoMoveShield == value)
                    return;

                Settings.NoMoveShield = value;
                EnsureCondition();
                Condition.NoMoveShield = value;
                OnPropertyChanged();
            }
        }

        public bool NoOriginShield
        {
            get => Settings.NoOriginShield;
            set
            {
                if (Settings.NoOriginShield == value)
                    return;

                Settings.NoOriginShield = value;
                EnsureCondition();
                Condition.NoOriginShield = value;
                OnPropertyChanged();
            }
        }

        private void EnsureCondition()
        {
            Condition ??= new CylinderCondition(() => true, () => true);
        }

        public abstract bool HaveAlarmReset { get; }

        public abstract bool IsOrigin { get; }

        public abstract bool IsMoved { get; }
        public abstract bool IsEnable { get; }

        public abstract bool IsOutMove { get; }
        public abstract bool IsOutOrigin { get; }

        public new abstract string Name { get; }
        public abstract void Enable();

        public abstract void Interlock();

        public abstract void ManualExecute();

        public abstract void Move();

        public abstract void Origin();

        public abstract void StatusJugger();
    }

    public class CylinderCondition
    {
        private readonly Func<bool> _offcondition;

        private readonly Func<bool> _oncondition;

        public bool NoOriginShield { get; set; }

        public bool NoMoveShield { get; set; }

        public External External { get; set; }

        public bool IsOffCondition
        {
            get
            {
                try
                {
                    return _offcondition();
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public bool IsOnCondition
        {
            get
            {
                try
                {
                    return _oncondition();
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public CylinderCondition(Func<bool> offCondition, Func<bool> onCondition)
        {
            _offcondition = offCondition;
            _oncondition = onCondition;
            External = new External();
        }
    }

    public class External : IExternal, IAlarmReset
    {
        public bool AirSignal { get; set; }

        public bool AlarmReset { get; set; }

        public bool ManualAutoMode { get; set; }

        public bool InitializingDone { get; set; }
    }

    public class CylinderDelay
    {
        public int OriginTime { get; set; }

        public int MoveTime { get; set; }

        public int AlarmTime { get; set; }

        public override string ToString()
        {
            return OriginTime + "," + MoveTime + "," + AlarmTime;
        }

        public static CylinderDelay Parse(string str)
        {
            string[] array = str.Split(',');
            CylinderDelay cylinderDelay = new CylinderDelay();
            cylinderDelay.OriginTime = int.Parse(array[0]);
            cylinderDelay.MoveTime = int.Parse(array[1]);
            cylinderDelay.AlarmTime = int.Parse(array[2]);
            return cylinderDelay;
        }
    }
}
