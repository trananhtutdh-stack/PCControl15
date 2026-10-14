using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using PCControl.Service.Config;
using PCControl.Service.IPC;
using PCControl.Service.DeviceManager.Actuator;
using PCControl.Service;
using PCControl.Service.Navigation;
using PCControl.Service.DeviceManager;
using PCControl.Machine;
using SharedLibrary.Wpf;
using AutoVision.Service.IPC;
using PCControl.DeviceManager;
namespace PCControl.ViewModel
{
    public partial class TeachingViewModel : ObservableObject
    {
        private readonly IActuatorDialogService _dialogService;

        public ObservableCollection<IAxis> AxisLst { get; set; }
        public ObservableCollection<Tool> ToolLst { get; set; }
        public ObservableCollection<Cylinder> CylinderLst { get; set; }
        public BlinkManager Blink => BlinkManager.Current;

        public MachineDevices Devices { get; }

        public PosSt PosZJigDown => PosControl.CH1_ROBOTZ_DOWN_JIG_POS;
        public PosSt PosZTrayDown => PosControl.CH1_ROBOTZ_DOWN_TRAY_POS;
        private PosSt CurToolXPos => CurTool.Index switch
        {
            1 => PosControl.CH1_ROBOTX_1ST_TOOL1_POS,
            2 => PosControl.CH1_ROBOTX_1ST_TOOL2_POS,
            3 => PosControl.CH1_ROBOTX_1ST_TOOL3_POS,
            4 => PosControl.CH1_ROBOTX_1ST_TOOL4_POS,
            _ => PosControl.CH1_ROBOTX_1ST_TOOL1_POS,
        };
        private PosSt CurToolYPos => CurTool.Index switch
        {
            1 => PosControl.CH1_ROBOTY_1ST_TOOL1_POS,
            2 => PosControl.CH1_ROBOTY_1ST_TOOL2_POS,
            3 => PosControl.CH1_ROBOTY_1ST_TOOL3_POS,
            4 => PosControl.CH1_ROBOTY_1ST_TOOL4_POS,
            _ => PosControl.CH1_ROBOTY_1ST_TOOL1_POS,
        };
        [ObservableProperty] private Tool _curTool;
        public double CurXJigPos
        {
            get => CurToolXPos.Position;
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
        public double JigTrayDeltaX => CurXJigPos - TrayXPos;
        public double JigTrayDeltaY => CurYJigPos - TrayYPos;
        partial void OnCurToolChanged(Tool value)
        {
            OnPropertyChanged(nameof(CurXJigPos));
            OnPropertyChanged(nameof(CurYJigPos));
            NotifyTeachingLayout();
        }

        private void NotifyTeachingLayout()
        {
            OnPropertyChanged(nameof(TrayXPos));
            OnPropertyChanged(nameof(TrayYPos));
            OnPropertyChanged(nameof(JigTrayDeltaX));
            OnPropertyChanged(nameof(JigTrayDeltaY));
        }
        public TeachingViewModel(MachineDevices devices, IActuatorDialogService dialogService)
        {
            Devices = devices;
            _dialogService = dialogService;

            AxisLst = new ObservableCollection<IAxis>
            {
                Devices.CH1_X_AXIS,
                Devices.CH1_Y_AXIS,
                Devices.CH1_Z_AXIS,
            };
            ToolLst = new ObservableCollection<Tool>
            {
                Devices.TOOL1_CH1,
                Devices.TOOL2_CH1,
                Devices.TOOL3_CH1,
                Devices.TOOL4_CH1,
            };
            CylinderLst = new ObservableCollection<Cylinder>
            {
                Devices.CYL_TOOL1_CH1,
                Devices.CYL_TOOL2_CH1,
                Devices.CYL_TOOL3_CH1,
                Devices.CYL_TOOL4_CH1,
            };
            _curTool = Devices.TOOL1_CH1;
        }

        #region MOTION
        [RelayCommand]
        public void ServoOn(object? obj)
        {
            if (obj is IAxis axis)
            {
                if (axis.IsServoON) axis.ServoOff();
                else axis.ServoOn();
            }
        }

        [RelayCommand]
        public void Home(object? obj)
        {
            if (obj is IAxis axis)
            {
                axis.Homing(axis.HomeSpeed, axis.CreepSpeed);
            }
        }
        [RelayCommand]
        public void JogP(object? obj)
        {
            if (obj is IAxis axis)
            {
                axis.JOG_Positive();
            }
        }
        [RelayCommand]
        public void JogN(object? obj)
        {
            if (obj is IAxis axis)
            {
                axis.JOG_Nagative();
            }
        }
        [RelayCommand]
        public void JogStop(object? obj)
        {
            if (obj is IAxis axis)
            {
                axis.JOG_STOP();
            }
        }
        [RelayCommand]
        public void Reset(object? obj)
        {
            if (obj is IAxis axis)
                axis.AxisErrClear();
        }
        [RelayCommand]
        public void JogLevel(object? obj)
        {
            if (obj is IAxis axis)
            {
                switch (axis.JogLevel)
                {
                    case 1:
                        axis.JogLevel = 2;
                        break;
                    case 2:
                        axis.JogLevel = 3;
                        break;
                    case 3:
                        axis.JogLevel = 1;
                        break;
                    default:
                        axis.JogLevel = 1;
                        break;
                }
            }
        }
        #endregion

        #region CYLINDER
        [RelayCommand]
        public void JigIn()
        {
            Devices.CYL_JIG_CH1.Move();
        }
        [RelayCommand]
        public void JigOut()
        {
            Devices.CYL_JIG_CH1.Origin();
        }
        [RelayCommand]
        public void TrayIn()
        {
            Devices.CYL_TRAY.Move();
        }
        [RelayCommand]
        public void TrayOut()
        {
            Devices.CYL_TRAY.Origin();
        }
        #endregion

        #region Tool
        [RelayCommand]
        public void ToolVaccum(object? obj)
        {
            if (obj is Tool tool)
            {
                tool.Vaccum();
            }
        }
        [RelayCommand]
        public void ToolBlow(object? obj)
        {
            if (obj is Tool tool)
            {
                tool.Blow();
            }
        }
        [RelayCommand]
        public void ToolOff(object? obj)
        {
            if (obj is Tool tool)
            {
                tool.Off();
            }
        }
        [RelayCommand]
        public void ToolUp(object? obj)
        {
            if (obj is Cylinder tool)
            {
                tool.Origin();
            }
        }
        [RelayCommand]
        public void ToolDown(object? obj)
        {
            if (obj is Cylinder tool)
            {
                tool.Move();
            }
        }
        [RelayCommand]
        private void SelectTool(object? obj)
        {
            if (obj is Tool tool)
                CurTool = tool;
        }
        #endregion

        #region JIG & TRAY
        [RelayCommand]
        public void CreatMatrix()
        {
            Devices.JigCh1.CreatePos(PosControl.CH1_ROBOTX_1ST_TOOL1_POS.Position, PosControl.CH1_ROBOTY_1ST_TOOL1_POS.Position, 10000, 10000, 0);
            Devices.JigCh1.CreatePos(PosControl.CH1_ROBOTX_1ST_TOOL2_POS.Position, PosControl.CH1_ROBOTY_1ST_TOOL2_POS.Position, 10000, 10000, 1);
            Devices.JigCh1.CreatePos(PosControl.CH1_ROBOTX_1ST_TOOL3_POS.Position, PosControl.CH1_ROBOTY_1ST_TOOL3_POS.Position, 10000, 10000, 2);
            Devices.JigCh1.CreatePos(PosControl.CH1_ROBOTX_1ST_TOOL4_POS.Position, PosControl.CH1_ROBOTY_1ST_TOOL4_POS.Position, 10000, 10000, 3);
            Devices.Tray.CreatePos(PosControl.CH1_ROBOTX_1ST_TRAY_POS.Position, PosControl.CH1_ROBOTY_1ST_TRAY_POS.Position, 1000, 1000, 1);
        }
        [RelayCommand]
        public void JigEdit()
        {
            _dialogService.ShowJigSetting(Devices.JigCh1);
        }

        [RelayCommand]
        public void TrayEdit()
        {
            _dialogService.ShowTraySetting(Devices.Tray);
        }
        [RelayCommand]
        private void SelectJigPos(JigItem item)
        {
            Devices.JigCh1.SelectJigPos(item.Index);
        }
        [RelayCommand]
        private void SelectTrayPos(TrayItem item)
        {
            Devices.Tray.SelectGroup(item.Index);
        }
        [RelayCommand]
        private void JigPosMov()
        {
            PosData pos = Devices.JigCh1.FindPos(Devices.JigCh1.TargetPos, CurTool.Index);
            Devices.CH1_X_AXIS.MoveAbs(pos.XPos, pos.XSpeed);
            Devices.CH1_Y_AXIS.MoveAbs(pos.YPos, pos.YSpeed);
        }
        [RelayCommand]
        private void TrayPosMov()
        {
            PosData pos = Devices.Tray.FindPos(Devices.Tray.TargetPos, 1);
            Devices.CH1_X_AXIS.MoveAbs(pos.XPos, pos.XSpeed);
            Devices.CH1_Y_AXIS.MoveAbs(pos.YPos, pos.YSpeed);
        }
        [RelayCommand]
        private void Z_JIG_DOWN()
        {
            //Devices.CH1_Z_AXIS.MoveAbs(PosZJigDown);
        }
        [RelayCommand]
        private void Z_TRAY_DOWN()
        {
            //Devices.CH1_Z_AXIS.MoveAbs(PosZTrayDown);
        }

        [RelayCommand]
        private void TeachJig1stFromRobot()
        {
            CurToolXPos.Position = Devices.CH1_X_AXIS.CurrentPos;
            CurToolYPos.Position = Devices.CH1_Y_AXIS.CurrentPos;
            OnPropertyChanged(nameof(CurXJigPos));
            OnPropertyChanged(nameof(CurYJigPos));
            NotifyTeachingLayout();
        }

        [RelayCommand]
        private void TeachTray1stFromRobot()
        {
            TrayXPos = Devices.CH1_X_AXIS.CurrentPos;
            TrayYPos = Devices.CH1_Y_AXIS.CurrentPos;
        }

        [RelayCommand]
        private void TeachZJigFromRobot()
        {
            PosZJigDown.Position = Devices.CH1_Z_AXIS.CurrentPos;
        }

        [RelayCommand]
        private void TeachZTrayFromRobot()
        {
            PosZTrayDown.Position = Devices.CH1_Z_AXIS.CurrentPos;
        }
        #endregion
    }
}
