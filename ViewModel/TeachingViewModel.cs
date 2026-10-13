using AutoVision.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Core.IPC;
using PCControl.Machine;
using PCControl.Service;
using PCControl.Service.DeviceManager.Actuator;

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PCControl.ViewModel
{
    public partial class TeachingViewModel : ObservableObject
    {
        
        public MachineDevices Devices { get; }
        private readonly MotionService _motionService;
        //private readonly PositionManager _positionManager;

        [ObservableProperty]
        private bool isCH1Selected = true;

        [ObservableProperty]
        private bool isCH2Selected;

        [ObservableProperty]
        private string selectedChannelName = "CH1";

        public ObservableCollection<IAxis> Axes { get; set; }

        public ObservableCollection<Cylinder1> Cylinders { get; set; }

        public ObservableCollection<Tool> Tools { get; set; }

        [ObservableProperty]
        private Tool _curTool;
        public PosSetting PosZJigDown => PosControl.CH1_ROBOTZ_DOWN_JIG_POS;

        public PosSetting PosZTrayDown => PosControl.CH1_ROBOTZ_DOWN_TRAY_POS;
        private PosSetting CurToolXPos => CurTool.Index switch
        {
            1 => PosControl.CH1_ROBOTX_1ST_TOOL1_POS,
            2 => PosControl.CH1_ROBOTX_1ST_TOOL2_POS,
            3 => PosControl.CH1_ROBOTX_1ST_TOOL3_POS,
            4 => PosControl.CH1_ROBOTX_1ST_TOOL4_POS,
            _ => PosControl.CH1_ROBOTX_1ST_TOOL1_POS,
        };

        private PosSetting CurToolYPos => CurTool.Index switch
        {
            1 => PosControl.CH1_ROBOTY_1ST_TOOL1_POS,
            2 => PosControl.CH1_ROBOTY_1ST_TOOL2_POS,
            3 => PosControl.CH1_ROBOTY_1ST_TOOL3_POS,
            4 => PosControl.CH1_ROBOTY_1ST_TOOL4_POS,
            _ => PosControl.CH1_ROBOTY_1ST_TOOL1_POS,
        };

        public double TrayXPos
        {
            get => PosControl.CH1_ROBOTX_1ST_TRAY_POS.Position;
            set
            {
                PosControl.CH1_ROBOTX_1ST_TRAY_POS.Position = value;
                OnPropertyChanged();
                NotifyTeachingLayout();
            }
        }
        public double TrayYPos
        {
            get => PosControl.CH1_ROBOTY_1ST_TRAY_POS.Position;
            set
            {
                PosControl.CH1_ROBOTY_1ST_TRAY_POS.Position = value;
                OnPropertyChanged();
                NotifyTeachingLayout();
            }
        }

        public double JigTrayDeltaX=> CurXJigPos-TrayXPos;

        public double JigTrayDeltaY => CurYJigPos - TrayYPos;

        public double CurXJigPos
        {
            get=> CurToolXPos.Position;
            set
            {
                CurToolXPos.Position = value;
                OnPropertyChanged();
                NotifyTeachingLayout();
            }
        }
        public double CurYJigPos
        {
            get => CurToolYPos.Position;
            set
            {
                CurToolYPos.Position = value;
                OnPropertyChanged();
                NotifyTeachingLayout();
            }
        }

        partial void OnSelectedToolChanged(Tool value)
        {
            
            OnPropertyChanged(nameof(CurXJigPos));
            OnPropertyChanged(nameof(CurYJigPos));
            //OnPropertyChanged(nameof(JigTrayDeltaX));
            //OnPropertyChanged(nameof(JigTrayDeltaY));
        }
        private void NotifyTeachingLayout()
        {
            OnPropertyChanged(nameof(TrayXPos));
            OnPropertyChanged(nameof(TrayYPos));
            OnPropertyChanged(nameof(JigTrayDeltaX));
            OnPropertyChanged(nameof(JigTrayDeltaY));
        }

        [ObservableProperty]
        private JIG? jigData;

        [ObservableProperty]
        private TRAY? trayData;

        //[ObservableProperty]
        //private Position3D? selectedPosition;

        [ObservableProperty]
        private Tool? selectedTool;

        [ObservableProperty]
        private Cylinder1? jigCylinder;

        [ObservableProperty]
        private Cylinder1? trayCylinder;

        public TeachingViewModel(
            MachineDevices devices,
            MotionService motionService
            //,PositionManager positionManager
            )
        {
            Devices = devices;
            _motionService = motionService;
            //_positionManager = positionManager;

            foreach (var axis in devices.Axis.Values)
            {
                axis.StatusJugger();
            }
            LoadChannel();

            //_curTool = Devices.Cylinders.["CYL_TOOL1_CH1"];
        }

        partial void OnIsCH1SelectedChanged(bool value)
        {
            if (!value)
                return;

            if (IsCH2Selected)
                IsCH2Selected = false;

            SelectedChannelName = "CH1";

            LoadChannel();
        }


        partial void OnIsCH2SelectedChanged(bool value)
        {
            if (!value)
                return;

            if (IsCH1Selected)
                IsCH1Selected = false;

            SelectedChannelName = "CH2";

            LoadChannel();
        }


        [RelayCommand]
        private void SelectCH1()
        {
            if (IsCH1Selected)
                return;

            IsCH1Selected = true;
        }


        [RelayCommand]
        private void SelectCH2()
        {
            if (IsCH2Selected)
                return;

            IsCH2Selected = true;
        }

        private void LoadChannel()
        {
            string prefix = IsCH1Selected
                ? "CH1_"
                : "CH2_";


            LoadAxes(prefix);

            LoadCylinders(prefix);

            LoadTools(prefix);

            //LoadPositionData();

            LoadSpecialCylinders(prefix);

            //SelectedPosition = null;
        }

        private void LoadAxes(string prefix)
        {
            Axes.Clear();

            foreach (var axis in Devices.Axis.Values)
            {
                if (!axis.Name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                axis.StatusJugger();

                Axes.Add(axis);
            }
        }

        private void LoadCylinders(string prefix)
        {
            Cylinders.Clear();

            foreach (var cylinder in Devices.Cylinders.Values)
            {
                if (!cylinder.Name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                cylinder.StatusJugger();

                Cylinders.Add(cylinder);
            }
        }

        private void LoadTools(string prefix)
        {
            Tools.Clear();

            foreach (var tool in Devices.Tools.Values)
            {
                if (!tool.Name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                tool.StatusJugger();

                Tools.Add(tool);
            }

            SelectedTool = Tools.FirstOrDefault();
        }


        //private void LoadPositionData()
        //{
        //    if (IsCH1Selected)
        //    {
        //        JigData = _positionManager.Positions.CH1.Jig;
        //        TrayData = _positionManager.Positions.CH1.Tray;
        //    }
        //    else
        //    {
        //        JigData = _positionManager.Positions.CH2.Jig;
        //        TrayData = _positionManager.Positions.CH2.Tray;
        //    }


        //    JigData?.RebuildPositions();
        //    TrayData?.RebuildPositions();


        //    OnPropertyChanged(nameof(JigData));
        //    OnPropertyChanged(nameof(TrayData));
        //}

        private void LoadSpecialCylinders(string prefix)
        {
            JigCylinder = null;
            TrayCylinder = null;


            foreach (var cylinder in Devices.Cylinders.Values)
            {
                if (!cylinder.Name.StartsWith(
                        prefix,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string name = cylinder.Name.ToUpperInvariant();

                if (name.Contains("JIG"))
                {
                    JigCylinder = cylinder;
                    continue;
                }

                if (name.Contains("TRAY"))
                {
                    TrayCylinder = cylinder;
                }
            }


            OnPropertyChanged(nameof(JigCylinder));
            OnPropertyChanged(nameof(TrayCylinder));
        }

        [RelayCommand]
        private void Servo(IAxis? axis)
        {
            if (axis == null)
                return;


            if (axis.IsServoON)
                _motionService.ServoOff(axis);
            else
                _motionService.ServoOn(axis);


            axis.StatusJugger();
        }

        [RelayCommand]
        private void JogPositive(IAxis? axis)
        {
            if (axis == null)
                return;

            if (!axis.CanMove)
                return;


            _motionService.JogPositive(axis);
        }

        [RelayCommand]
        private void JogNegative(IAxis? axis)
        {
            if (axis == null)
                return;

            if (!axis.CanMove)
                return;


            _motionService.JogNegative(axis);
        }

        [RelayCommand]
        private void JogStop(IAxis? axis)
        {
            if (axis == null)
                return;


            _motionService.JogStop(axis);

            axis.StatusJugger();
        }

        [RelayCommand]
        private void Home(IAxis? axis)
        {
            if (axis == null)
                return;

            if (!axis.CanMove)
                return;


            _motionService.Home(axis);

            axis.StatusJugger();
        }


        [RelayCommand]
        private void ClearError(IAxis? axis)
        {
            if (axis == null)
                return;


            _motionService.ClearError(axis);

            axis.StatusJugger();
        }


        [RelayCommand]
        private void JogHigh(IAxis? axis)
        {
            if (axis == null)
                return;
            axis.JogLevel = 3;
            axis.JogSpeed = axis.JogHSpeed;
            axis.StatusJugger();
        }

        [RelayCommand]
        private void JogMedium(IAxis? axis)
        {
            if (axis == null)
                return;
            axis.JogLevel = 2;
            axis.JogSpeed = axis.JogMSpeed;
            axis.StatusJugger();
        }

        [RelayCommand]
        private void JogLow(IAxis? axis)
        {
            if (axis == null)
                return;
            axis.JogLevel = 1;
            axis.JogSpeed = axis.JogLSpeed;
            axis.StatusJugger();
        }

        [RelayCommand]
        private void StopAxis(IAxis? axis)
        {
            if (axis == null)
                return;
            _motionService.JogStop(axis);
            axis.StatusJugger();
        }

        [RelayCommand]
        private void StopAll()
        {
            foreach (var axis in Axes)
            {
                _motionService.JogStop(axis);

                axis.StatusJugger();
            }
        }

        [RelayCommand]
        private void MoveAbsolute(IAxis? axis)
        {
            if (axis == null)
                return;
            if (!axis.CanMove)
                return;
            if (axis.Speed <= 0)
                return;
            _motionService.MoveAbsolute(
                axis,
                axis.TargetPos,
                axis.Speed);
            axis.StatusJugger();
        }

        [RelayCommand]
        private void MoveRelative(IAxis? axis)
        {
            if (axis == null)
                return;
            if (!axis.CanMove)
                return;
            if (axis.Speed <= 0)
                return;
            _motionService.MoveRelative(axis, axis.TargetPos, axis.Speed);
            axis.StatusJugger();
        }

        //[RelayCommand]
        //private void SelectJigPosition(Position3D? position)
        //{
        //    if (position == null)
        //        return;


        //    SelectedPosition = position;
        //}

        //[RelayCommand]
        //private void SelectTrayPosition(Position3D? position)
        //{
        //    if (position == null)
        //        return;
        //    SelectedPosition = position;
        //}

        //[RelayCommand]
        //private void TeachSelectedPosition()
        //{
        //    //if (SelectedPosition == null)
        //    //    return;
        //    var x = GetAxis("_X_AXIS");
        //    var y = GetAxis("_Y_AXIS");
        //    var z = GetAxis("_Z_AXIS");
        //    if (x == null ||
        //        y == null ||
        //        z == null)
        //    {
        //        return;
        //    }

        //    SelectedPosition.X = x.CurrentPos;
        //    SelectedPosition.Y = y.CurrentPos;
        //    SelectedPosition.Z = z.CurrentPos;
        //    OnPropertyChanged(nameof(SelectedPosition));
        //    //_positionManager.Save();
        //}

        [RelayCommand]
        private void TeachJig()
        {
            if (JigData == null)
                return;
            var x = GetAxis("_X_AXIS");
            var y = GetAxis("_Y_AXIS");
            var z = GetAxis("_Z_AXIS");
            if (x == null ||
                y == null ||
                z == null)
            {
                return;
            }
            //JigData.FirstPosition.X = x.CurrentPos;
            //JigData.FirstPosition.Y = y.CurrentPos;
            //JigData.FirstPosition.Z = z.CurrentPos;
            //JigData.RebuildPositions();
            //OnPropertyChanged(nameof(JigData));
            //_positionManager.Save();
        }

        //[RelayCommand]
        //private void TeachTray()
        //{
        //    if (TrayData == null)
        //        return;
        //    var x = GetAxis("_X_AXIS");
        //    var y = GetAxis("_Y_AXIS");
        //    var z = GetAxis("_Z_AXIS");
        //    if (x == null ||
        //        y == null ||
        //        z == null)
        //    {
        //        return;
        //    }
        //    TrayData.FirstPosition.X = x.CurrentPos;
        //    TrayData.FirstPosition.Y = y.CurrentPos;
        //    TrayData.FirstPosition.Z = z.CurrentPos;
        //    TrayData.RebuildPositions();
        //    OnPropertyChanged(nameof(TrayData));
        //    _positionManager.Save();
        //}

        //[RelayCommand]
        //private void TeachZJig()
        //{
        //    if (JigData == null)
        //        return;
        //    var z = GetAxis("_Z_AXIS");

        //    if (z == null)
        //        return;
        //    JigData.FirstPosition.Z = z.CurrentPos;
        //    JigData.RebuildPositions();
        //    OnPropertyChanged(nameof(JigData));
        //    _positionManager.Save();
        //}

        //[RelayCommand]
        //private void TeachZTray()
        //{
        //    if (TrayData == null)
        //        return;
        //    var z = GetAxis("_Z_AXIS");
        //    if (z == null)
        //        return;
        //    TrayData.FirstPosition.Z = z.CurrentPos;
        //    TrayData.RebuildPositions();
        //    OnPropertyChanged(nameof(TrayData));
        //    _positionManager.Save();
        //}

        //[RelayCommand]
        //private void MoveJigFirst()
        //{
        //    if (JigData == null)
        //        return;
        //    MovePosition(JigData.FirstPosition);
        //}

        //[RelayCommand]
        //private void MoveTrayFirst()
        //{
        //    if (TrayData == null)
        //        return;
        //    MovePosition(TrayData.FirstPosition);
        //}

        //[RelayCommand]
        //private void MoveJigZ()
        //{
        //    if (JigData == null)
        //        return;
        //    MoveAxisZ(JigData.FirstPosition.Z);
        //}

        //[RelayCommand]
        //private void MoveTrayZ()
        //{
        //    if (TrayData == null)
        //        return;
        //    MoveAxisZ(TrayData.FirstPosition.Z);
        //}

        //private void MovePosition(Position3D position)
        //{
        //    var x = GetAxis("_X_AXIS");
        //    var y = GetAxis("_Y_AXIS");
        //    var z = GetAxis("_Z_AXIS");
        //    if (x == null ||
        //        y == null ||
        //        z == null)
        //    {
        //        return;
        //    }
        //    if (!CanMove(x) ||
        //        !CanMove(y) ||
        //        !CanMove(z))
        //    {
        //        return;
        //    }
        //    double xSpeed = GetMoveSpeed(x);
        //    double ySpeed = GetMoveSpeed(y);
        //    double zSpeed = GetMoveSpeed(z);
        //    _motionService.MoveAbsolute(
        //        x,
        //        position.X,
        //        xSpeed);

        //    _motionService.MoveAbsolute(
        //        y,
        //        position.Y,
        //        ySpeed);

        //    _motionService.MoveAbsolute(
        //        z,
        //        position.Z,
        //        zSpeed);

        //    x.StatusJugger();
        //    y.StatusJugger();
        //    z.StatusJugger();
        //}

        private void MoveAxisZ(double position)
        {
            var z = GetAxis("_Z_AXIS");

            if (z == null)
                return;

            if (!CanMove(z))
                return;

            _motionService.MoveAbsolute(
                z,
                position,
                GetMoveSpeed(z));

            z.StatusJugger();
        }

        private IAxis? GetAxis(string suffix)
        {
            return Axes.FirstOrDefault(
                axis => axis.Name.EndsWith(
                    suffix,
                    StringComparison.OrdinalIgnoreCase));
        }

        private static bool CanMove(IAxis axis)
        {
            //if (!axis.IsServoON)
            //    return false;

            if (!axis.CanMove)
                return false;

            //if (axis.DevAlarm)
            //    return false;

            return true;
        }

        private static double GetMoveSpeed(IAxis axis)
        {
            if (axis.Speed > 0)
                return axis.Speed;

            if (axis.JogSpeed > 0)
                return axis.JogSpeed;

            return axis.JogLSpeed;
        }

        //[RelayCommand]
        //private async Task JigForwardAsync()
        //{
        //    if (JigCylinder == null)
        //        return;


        //    await JigCylinder.ForwardAsync(
        //        CancellationToken.None);
        //}

        //[RelayCommand]
        //private async Task JigBackwardAsync()
        //{
        //    if (JigCylinder == null)
        //        return;

        //    await JigCylinder.BackwardAsync(
        //        CancellationToken.None);
        //}

        //[RelayCommand]
        //private void JigStop()
        //{
        //    JigCylinder?.Stop();
        //}

        //[RelayCommand]
        //private async Task TrayForwardAsync()
        //{
        //    if (TrayCylinder == null)
        //        return;
        //    await TrayCylinder.ForwardAsync(
        //        CancellationToken.None);
        //}

        //[RelayCommand]
        //private async Task TrayBackwardAsync()
        //{
        //    if (TrayCylinder == null)
        //        return;

        //    await TrayCylinder.BackwardAsync(
        //        CancellationToken.None);
        //}

        //[RelayCommand]
        //private void TrayStop()
        //{
        //    TrayCylinder?.Stop();
        //}

        [RelayCommand]
        private void SelectTool(Tool? tool)
        {
            if (tool == null)
                return;


            SelectedTool = tool;
        }

        [RelayCommand]
        private void Vacuum(Tool? tool)
        {
            if (tool == null)
                return;

            tool.Vaccum();

            tool.StatusJugger();
        }

        [RelayCommand]
        private void Blow(Tool? tool)
        {
            if (tool == null)
                return;


            tool.Blow();

            tool.StatusJugger();
        }

        [RelayCommand]
        private void ToolOff(Tool? tool)
        {
            if (tool == null)
                return;

            tool.Off();

            tool.StatusJugger();
        }

        [RelayCommand]
        private void OneCycle()
        {
            // Teaching chỉ thực hiện manual test.

        }
    }
}