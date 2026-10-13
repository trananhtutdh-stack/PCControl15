

using AutoVision.Service.IPC;
using PCControl.Machine;
using PCControl.Views;

namespace PCControl.Sequence
{
    public class SeqStatus : SeqBase
    {
        private readonly SeqThread _seqThread;
        public enum eStep
        {
            IDLE = -1,
            UPDATE_STATUS = 0,
        }
        public SeqStatus(SeqThread seqTh) : base(seqTh)
        {
            _seqThread = seqTh;
        }
        public override void Run()
        {
            if(false == GetWork())
                return;
            switch ((eStep)Step)
            {
                case eStep.IDLE:


                    NextStep();
                    break;
                case eStep.UPDATE_STATUS:
                    foreach (IAxis axis in _seqThread.Devices.Axis.Values)
                    {
                        axis.StatusJugger();
                    }


                    Delay(50);

                    MoveStep((int)eStep.IDLE);
                    break;
            }

        }

    }
}
