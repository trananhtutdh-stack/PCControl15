using AutoVision.Service.IPC;
using PCControl.Service.IPC;

namespace PCControl.Service
{
    public enum HButtonMode
    {
        NO = 1,
        NC = 2,
    }
    public class MButton
    {
        private IIoPoint _button;
        private IIoPoint? _lamp;

        public bool isActive => _button.Value;
        public bool isLampON
        {
            get
            {
                if(_lamp != null)
                    return _lamp.Value;
                else return false;
            }
        }

        public void SetLamp()
        {
            if(_lamp == null) return;
            _lamp.Value = true;
        }
        public void RstLamp()
        {
            if(_lamp == null) return;
            _lamp.Value = false;
        }
        public MButton(IIoPoint button, IIoPoint lamp, HButtonMode mode) 
        {
            _button = button;
            _lamp = lamp;
        
        }
        public MButton(IIoPoint button, HButtonMode mode)
        {
            _button = button;

        }

        public void VerifyIoPoint(IoModes ioMode)
        {
            if (_button.IoMode == IoModes.output)
            {
                throw new InvalidOperationException(string.Format("Button must be Input"));
            }
            if (_lamp == null) return;
            if (_lamp.IoMode == IoModes.input)
            {
                throw new InvalidOperationException(string.Format("Lamp must be Output"));
            }
        }
    }
}
