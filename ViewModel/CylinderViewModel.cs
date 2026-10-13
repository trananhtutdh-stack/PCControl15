using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PCControl.Machine;
using PCControl.Service;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace PCControl.ViewModel
{
    public partial class CylinderViewModel : ObservableObject
    {
        private readonly Dictionary<string, CancellationTokenSource> _cts = new();

        private readonly DispatcherTimer _timer;
        public ObservableCollection<CylinderItem> Cylinders { get; } = new();

        public CylinderViewModel(MachineDevices devices)
        {
            foreach (var cylinder in devices.Cylinders.Values)
            {
                Cylinders.Add(new CylinderItem(cylinder));
            }

            _timer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(10)
            };
            _timer.Tick += UpdateCylinders;
            _timer.Start();
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task Forward(CylinderItem item)
        {
            CancelCurrent(item);
            var cts = new CancellationTokenSource();
            _cts[item.Name] = cts;
            try
            {
                item.Status = "Forwarding...";
                bool result = await item.Cylinder.ForwardAsync(cts.Token);
                item.Update();
                if (!result)
                {
                    item.Status = "Timeout";
                }
            }
            catch (OperationCanceledException)
            {
                item.Update();
            }
        }

        [RelayCommand(AllowConcurrentExecutions = true)]
        private async Task Backward(CylinderItem item)
        {
            CancelCurrent(item);
            var cts = new CancellationTokenSource();
            _cts[item.Name] = cts;
            try
            {
                item.Status = "Backwarding...";
                bool result = await item.Cylinder.BackwardAsync(cts.Token);
                item.Update();
                if (!result)
                {
                    item.Status = "Timeout";
                }
            }
            catch (OperationCanceledException)
            {
                item.Update();
            }
        }

        [RelayCommand]
        private void Stop(CylinderItem item)
        {
            CancelCurrent(item);
            item.Cylinder.Stop();
            item.Update();
        }

        [RelayCommand]
        private void Cancel(CylinderItem item)
        {
            CancelCurrent(item);
            item.Update();
        }

        private void UpdateCylinders(object? sender,EventArgs e)
        {
            foreach (var item in Cylinders)
            {
                item.Update();
            }
        }

        private void CancelCurrent(CylinderItem item)
        {
            if (_cts.TryGetValue(item.Name,out var cts))
            {
                cts.Cancel();
                cts.Dispose();
                _cts.Remove(item.Name);
            }
        }

        public partial class CylinderItem : ObservableObject
        {
            public Cylinder1 Cylinder { get; }
            public string Name => Cylinder.Name;

            [ObservableProperty]
            private string status;

            [ObservableProperty]
            private bool forwardSensor;

            [ObservableProperty]
            private bool backwardSensor;

            [ObservableProperty]
            private bool bypassForwardSensor;

            [ObservableProperty]
            private bool bypassBackwardSensor;

            public CylinderItem(Cylinder1 cylinder)
            {
                Cylinder = cylinder;
                status = cylinder.State.ToString();
                Update();
            }

            public void Update()
            {
                ForwardSensor = Cylinder.ForwardSensor;

                BackwardSensor = Cylinder.BackwardSensor;

                Status = Cylinder.State.ToString();

                BypassForwardSensor = Cylinder.BypassForwardSensor;

                BypassBackwardSensor = Cylinder.BypassBackwardSensor;
            }

            partial void OnBypassForwardSensorChanged(bool value)
            {
                Cylinder.BypassForwardSensor = value;
            }

            partial void OnBypassBackwardSensorChanged(bool value)
            {
                Cylinder.BypassBackwardSensor = value;
            }
        }
    }
}