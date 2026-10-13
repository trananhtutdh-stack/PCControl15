using System.ComponentModel;
using System.Diagnostics;
using AutoVision.Service.IPC;
using PCControl.Service.Config;

namespace PCControl.Service.IPC
{
    public class SingleCylinder : Cylinder
    {
        private string name;

        public DevAlarm DevAlarm
        {
            get
            {
                DevAlarm al = null;
                if (NoOriginAlarm())
                    al = new DevAlarm(() => true)
                    {
                        AlarmLevel = AlarmLevels.Error,
                        Message = ((Language == "en") ? String.Format("No Origin Sensor {0} Alarm", _InOrigin.AdressName) : String.Format("Không có tín hiệu sensor {0} khi về gốc", _InOrigin.AdressName)),
                        Solution = ((Language == "en") ? String.Format("Check {0} Signal ", _InOrigin.AdressName) : String.Format("Kiểm tra tín hiệu sensor {0}", _InOrigin.AdressName))
                    };
                if (NoMoveAlarm())
                    al = new DevAlarm(() => true)
                    {
                        AlarmLevel = AlarmLevels.Error,
                        Message = ((Language == "en") ? String.Format("No Move Sensor {0} Alarm", _InMove.AdressName) : String.Format("Không có tín hiệu sensor {0} khi cylinder di chuyển", _InMove.AdressName)),
                        Solution = ((Language == "en") ? String.Format("Check {0} Signal ", _InMove.AdressName) : String.Format("Kiểm tra tín hiệu sensor {0}", _InMove.AdressName))
                    };
                if (OnOriginAlarm())
                    al = new DevAlarm(() => true)
                    {
                        AlarmLevel = AlarmLevels.Error,
                        Message = ((Language == "en") ? String.Format("On Origin Sensor {0} Alarm", _InOrigin.AdressName) : String.Format("Có tín hiệu sensor {0} khi cylinder đã di chuyển", _InOrigin)),
                        Solution = ((Language == "en") ? String.Format("Check {0} Signal ", _InOrigin.AdressName) : String.Format("Kiểm tra tín hiệu sensor {0}", _InOrigin.AdressName))
                    };
                if (OffMoveAlarm())
                    al = new DevAlarm(() => true)
                    {
                        AlarmLevel = AlarmLevels.Error,
                        Message = ((Language == "en") ? String.Format("Off Move Sensor {0} Alarm", _InMove) : String.Format("Có tín hiệu sensor {0} khi cylinder không di chuyển", _InMove)),
                        Solution = ((Language == "en") ? String.Format("Check {0} Signal ", _InMove.AdressName) : String.Format("Kiểm tra tín hiệu sensor {0}", _InMove.AdressName))
                    };
                return al;
            }
        }

        private bool _noMoveAlarm;

        private bool _noOriginAlarm;

        private bool _offMoveAlarm;

        private bool _onOriginAlarm;

        private bool cylinderNoOrigin;

        private bool cylinderNoMove;

        private bool cylinderOffMove;

        private bool cylinderOnOrigin;

        private bool _OutOriginStatus;

        private bool _OutMoveStatus;

        private IIoPoint _InOrigin;

        private IIoPoint _InMove;

        private IIoPoint _OutMove;

        private bool cylinderManual;

        private bool cylinderEnable;

        private bool _autoExecute;

        private readonly Stopwatch _watchOrigin = new Stopwatch();

        private readonly Stopwatch _watchMove = new Stopwatch();

        private readonly Stopwatch[] _watchAlarm = new Stopwatch[4]
        {
        new Stopwatch(),
        new Stopwatch(),
            new Stopwatch(),
        new Stopwatch()
        };

        public string Language { get; set; } = "";

        public override bool HaveAlarmReset
        {
            get
            {
                lock (this)
                {
                    if ((_noOriginAlarm && !cylinderNoOrigin) || (_noMoveAlarm && !cylinderNoMove) || (_offMoveAlarm && !cylinderOffMove) || (_onOriginAlarm && !cylinderOnOrigin))
                    {
                        return true;
                    }

                    return false;
                }
            }
        }

        public override bool IsEnable => cylinderEnable;

        public override bool IsMoved => _OutMoveStatus;

        public override bool IsOrigin => _OutOriginStatus;

        public override bool IsOutMove => _OutMove.Value;
        public override bool IsOutOrigin => !_OutMove.Value;
        public override string Name => name;

        public SingleCylinder(IIoPoint InOrigin, IIoPoint InMove, IIoPoint OutMove, string name, CylinderSt settings) : base(settings, name)
        {
            _InOrigin = InOrigin;
            _InMove = InMove;
            _OutMove = OutMove;
            _watchOrigin.Restart();
            _watchMove.Restart();
            cylinderEnable = true;
            this.name = name;
        }

        private bool NoOriginAlarm()
        {
            bool flag = false;
            if (!_OutMove.Value && !_InOrigin.Value)
            {
                cylinderNoOrigin = true;
                _watchAlarm[0].Start();
            }
            else
            {
                cylinderNoOrigin = false;
            }

            if (cylinderNoOrigin && !NoOriginShield)
            {
                flag = ((AlarmTime <= _watchAlarm[0].ElapsedMilliseconds) ? true : false);
            }
            else
            {
                _watchAlarm[0].Restart();
            }

            if ((cylinderNoOrigin && flag) || (_noOriginAlarm && !base.Condition.External.AlarmReset))
            {
                _noOriginAlarm = true;
            }
            else
            {
                _noOriginAlarm = false;
            }

            return _noOriginAlarm;
        }

        private bool NoMoveAlarm()
        {
            bool flag = false;
            if (base.Condition.External.AirSignal && _OutMove.Value && !_InMove.Value)
            {
                cylinderNoMove = true;
            }
            else
            {
                cylinderNoMove = false;
            }

            if (cylinderNoMove && !NoMoveShield)
            {
                flag = ((AlarmTime <= _watchAlarm[1].ElapsedMilliseconds) ? true : false);
            }
            else
            {
                _watchAlarm[1].Restart();
            }

            if ((cylinderNoMove && flag) || (_noMoveAlarm && !base.Condition.External.AlarmReset))
            {
                _noMoveAlarm = true;
            }
            else
            {
                _noMoveAlarm = false;
            }

            return _noMoveAlarm;
        }

        private bool OffMoveAlarm()
        {
            bool flag = false;
            if (base.Condition.External.AirSignal && !_OutMove.Value && _InMove.Value)
            {
                cylinderOffMove = true;
            }
            else
            {
                cylinderOffMove = false;
            }

            if (cylinderOffMove && !NoMoveShield)
            {
                flag = ((AlarmTime <= _watchAlarm[2].ElapsedMilliseconds) ? true : false);
            }
            else
            {
                _watchAlarm[2].Restart();
            }

            if ((cylinderOffMove && flag) || (_offMoveAlarm && !base.Condition.External.AlarmReset))
            {
                _offMoveAlarm = true;
            }
            else
            {
                _offMoveAlarm = false;
            }

            return _offMoveAlarm;
        }

        private bool OnOriginAlarm()
        {
            bool flag = false;
            if (base.Condition.External.AirSignal && _OutMove.Value && _InOrigin.Value)
            {
                cylinderOnOrigin = true;
            }
            else
            {
                cylinderOnOrigin = false;
            }

            if (cylinderOnOrigin && !NoOriginShield)
            {
                flag = ((AlarmTime <= _watchAlarm[3].ElapsedMilliseconds) ? true : false);
            }
            else
            {
                _watchAlarm[3].Restart();
            }

            if ((cylinderOnOrigin && flag) || (_onOriginAlarm && !base.Condition.External.AlarmReset))
            {
                _onOriginAlarm = true;
            }
            else
            {
                _onOriginAlarm = false;
            }

            return _onOriginAlarm;
        }

        private bool AutoOnPrompt()
        {
            if (base.Condition.External.ManualAutoMode && base.Condition.External.InitializingDone && cylinderManual)
            {
                return true;
            }

            return false;
        }

        public override void Enable()
        {
            cylinderEnable = true;
        }

        public override void Interlock()
        {
            cylinderEnable = false;
        }

        public override void ManualExecute()
        {
            if (!base.Condition.External.ManualAutoMode)
            {
                if (!cylinderManual)
                {
                    if ((!_autoExecute && base.Condition.IsOnCondition) || (_autoExecute && base.Condition.IsOffCondition))
                    {
                        cylinderManual = true;
                    }
                }
                else if ((!_autoExecute && base.Condition.IsOffCondition) || (_autoExecute && base.Condition.IsOnCondition))
                {
                    cylinderManual = false;
                }
            }

            cylinderEnable = true;
            CylinderActive();
        }

        public override void Move()
        {
            if (!cylinderManual)
            {
                _autoExecute = true;
                CylinderActive();
            }
            OnPropertyChanged(nameof(IsOutMove));
            OnPropertyChanged(nameof(IsOutOrigin));
        }

        public override void Origin()
        {
            if (!cylinderManual)
            {
                _autoExecute = false;
                CylinderActive();
            }
            OnPropertyChanged(nameof(IsOutMove));
            OnPropertyChanged(nameof(IsOutOrigin));
        }

        private void CylinderActive()
        {
            if (((cylinderManual && !_autoExecute) || (!cylinderManual && _autoExecute)) && cylinderEnable)
            {
                _OutMove.Value = true;
            }
            else
            {
                _OutMove.Value = false;
            }
        }

        public override void StatusJugger()
        {
            bool oldOrigin = _OutOriginStatus;

            bool oldMove = _OutMoveStatus;

            if (!_OutMove.Value &&
               ((_InOrigin.Value && !NoOriginShield)
               || NoOriginShield))
            {
                _watchOrigin.Stop();

                if (OriginTime <= _watchOrigin.ElapsedMilliseconds)
                {
                    _OutOriginStatus = true;
                }
                else
                {
                    _OutOriginStatus = false;
                }

                _watchOrigin.Start();
            }
            else
            {
                _watchOrigin.Restart();

                _OutOriginStatus = false;
            }

            if (_OutMove.Value &&
               ((_InMove.Value && !NoMoveShield)
               || NoMoveShield))
            {
                _watchMove.Stop();

                if (MoveTime <= _watchMove.ElapsedMilliseconds)
                {
                    _OutMoveStatus = true;
                }
                else
                {
                    _OutMoveStatus = false;
                }

                _watchMove.Start();
            }
            else
            {
                _watchMove.Restart();

                _OutMoveStatus = false;
            }

            if (oldOrigin != _OutOriginStatus)
                OnPropertyChanged(nameof(IsOrigin));

            if (oldMove != _OutMoveStatus)
                OnPropertyChanged(nameof(IsMoved));
        }
    }
}
