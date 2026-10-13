using AutoVision.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Service;
using PCControl.Views;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using ShareLibrary.Localizations;
using PCControl.Machine;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

namespace PCControl.ViewModel
{
    public partial class MotionViewModel : ObservableObject
    {
        private readonly MotionService _motionService;
        private readonly MachineDevices _machineDevices;
        public static ObservableCollection<IAxis> Axes { get; } = new();
        public MotionViewModel(MotionService motionService, MachineDevices devices)
        {
            _motionService = motionService;
            _machineDevices = devices;
            foreach (var axis in devices.Axis.Values)
            {
                axis.StatusJugger();
                Axes.Add(axis);
            }
            

            _motionService.ServoOn(devices.Axis["CH1_X_AXIS"]);
            _motionService.ServoOn(devices.Axis["CH1_Y_AXIS"]);
            _motionService.ServoOn(devices.Axis["CH1_Z_AXIS"]);
            _motionService.ServoOn(devices.Axis["CH2_X_AXIS"]);
            _motionService.ServoOn(devices.Axis["CH2_Y_AXIS"]);
            _motionService.ServoOn(devices.Axis["CH2_Z_AXIS"]);
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
            foreach (IAxis axis in Axes)
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
            _motionService.MoveAbsolute(axis,axis.TargetPos,axis.Speed);
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
            _motionService.MoveRelative(axis,axis.TargetPos,axis.Speed);
            axis.StatusJugger();
        }

        

    }
}