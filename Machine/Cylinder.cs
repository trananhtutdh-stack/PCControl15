using System;
using System.Threading;
using System.Threading.Tasks;
using AutoVision.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PCControl.Service
{
    public enum CylinderType
    {
        SingleActing,
        DoubleActing
    }

    public enum CylinderState
    {
        Unknown,
        Forwarding,
        Forward,
        Backwarding,
        Backward,
        Stopped,
        Timeout,
        Fault
    }

    public class Cylinder1:ObservableObject
    {
        private readonly IIpc _ipc;
        private readonly string _name;
        private readonly string _forwardOutput;
        private readonly string? _backwardOutput;
        private readonly string? _forwardSensor;
        private readonly string? _backwardSensor;
        private readonly CylinderType _type;
        private readonly int _forwardTimeoutMs;
        private readonly int _backwardTimeoutMs;
        private bool _bypassForwardSensor;
        private bool _bypassBackwardSensor;
        public string Name => _name;

        private CylinderState _state = CylinderState.Unknown;

        public CylinderState State
        {
            get => _state;
            private set => SetProperty(ref _state, value);
        }
        public bool BypassForwardSensor
        {
            get => _bypassForwardSensor;
            set => _bypassForwardSensor = value;
        }

        public bool BypassBackwardSensor
        {
            get => _bypassBackwardSensor;
            set => _bypassBackwardSensor = value;
        }

        public void StatusJugger()
        {
            OnPropertyChanged(nameof(ForwardSensor));
            OnPropertyChanged(nameof(BackwardSensor));
            OnPropertyChanged(nameof(IsForward));
            OnPropertyChanged(nameof(IsBackward));
        }


        public Cylinder1(string name, string forwardOutput, string? backwardOutput, string? forwardSensor, string? backwardSensor, IIpc ipc, CylinderType type = CylinderType.DoubleActing, int forwardTimeoutMs = 3000, int backwardTimeoutMs = 3000)
        {
            _name = name;
            _forwardOutput = forwardOutput;
            _backwardOutput = backwardOutput;
            _forwardSensor = forwardSensor;
            _backwardSensor = backwardSensor;
            _ipc = ipc;
            _type = type;
            _forwardTimeoutMs = forwardTimeoutMs;
            _backwardTimeoutMs = backwardTimeoutMs;
        }
        public bool ForwardSensor
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_forwardSensor))
                    return false;
                return ReadInput(_forwardSensor);
            }
        }

        public bool BackwardSensor
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_backwardSensor))
                    return false;
                return ReadInput(_backwardSensor);
            }
        }
        public bool IsForward => ForwardSensor;

        public bool IsBackward => BackwardSensor;

        public async Task<bool> ForwardAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                if (_type == CylinderType.DoubleActing)
                {
                    WriteOutput(_backwardOutput,false);
                }
                WriteOutput(_forwardOutput, true);
                State = CylinderState.Forwarding;
                if (!NeedForwardSensor())
                {
                    State = CylinderState.Forward;
                    return true;
                }
                DateTime start = DateTime.UtcNow;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (ForwardSensor)
                    {
                        State = CylinderState.Forward;
                        return true;
                    }
                    if ((DateTime.UtcNow - start).TotalMilliseconds >= _forwardTimeoutMs)
                    {
                        Stop();
                        State = CylinderState.Timeout;
                        return false;
                    }
                    await Task.Delay(20,cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Stop();
                State = CylinderState.Stopped;
                throw;
            }
        }

        public async Task<bool> BackwardAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                WriteOutput(_forwardOutput,false);
                if (_type == CylinderType.DoubleActing)
                {
                    WriteOutput(_backwardOutput,true);
                }
                State = CylinderState.Backwarding;
                if (!NeedBackwardSensor())
                {
                    State = CylinderState.Backward;
                    return true;
                }
                DateTime start = DateTime.UtcNow;
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    if (BackwardSensor)
                    {
                        State = CylinderState.Backward;
                        return true;
                    }
                    if ((DateTime.UtcNow - start).TotalMilliseconds >= _backwardTimeoutMs)
                    {
                        Stop();
                        State = CylinderState.Timeout;
                        return false;
                    }
                    await Task.Delay(20,cancellationToken);
                }
            }
            catch (OperationCanceledException)
            {
                Stop();
                State = CylinderState.Stopped;
                throw;
            }
        }

        public void Stop()
        {
            WriteOutput(_forwardOutput,false);
            WriteOutput(_backwardOutput,false);
            State = CylinderState.Stopped;
        }

        private bool NeedForwardSensor()
        {
            if (string.IsNullOrWhiteSpace(_forwardSensor))
                return false;
            if (_bypassForwardSensor)
                return false;
            return true;
        }

        private bool NeedBackwardSensor()
        {
            if (string.IsNullOrWhiteSpace(_backwardSensor))
                return false;
            if (_bypassBackwardSensor)
                return false;
            return true;
        }

        private bool ReadInput(string address)
        {
            if (!TryParseAddress(address,out int slave,out int bit))
            {
                State = CylinderState.Fault;
                return false;
            }
            return _ipc.IO.ReadInput(slave,bit);
        }

        private void WriteOutput(string? address,bool value)
        {
            if (string.IsNullOrWhiteSpace(address))
                return;
            if (!TryParseAddress(address,out int slave,out int bit))
            {
                State = CylinderState.Fault;
                return;
            }
            _ipc.IO.WriteOutput(slave,bit,value);
        }

        private bool TryParseAddress(string address,out int slave,out int bit)
        {
            slave = 0;
            bit = 0;
            if (string.IsNullOrWhiteSpace(address))
                return false;

            address = address.Trim().ToUpper();

            if (address.Length < 2)
                return false;
            char type = address[0];

            if (type != 'X' && type != 'Y')
                return false;
            if (!int.TryParse(address.Substring(1),out int number))
            {
                return false;
            }
            slave = number / 10;
            bit = number % 10;
            return bit >= 0 && bit <= 7;
        }
    }
}