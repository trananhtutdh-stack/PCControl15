
using System.Windows;
using PCControl.Machine;

namespace PCControl.Sequence
{
    public class Seq_LoadTray : SeqBase
    {
        
        public enum eStep
        {
            IDLE = -1,
            XY_MOVE_TO_X2Y2_CYL2_MOVING,
            XY_MOVE_TO_X2Y2_OK,
            Z_MOVE_TO_Z2,
            Z_MOVE_TO_Z2_OK,
            VACUUM_OFF,
            WAIT_X11_ON,
            Z_MOVE_TO_Z1_WAIT_POS,
            Z1_WAIT_POS_OK,
            CYL2_ORG
        }
        public Seq_LoadTray(SeqThread seqTh) : base(seqTh)
        {
            
        }

        public override void Run()
        {
            if (false == GetWork())
                return;
            switch ((eStep)Step)
            {
                //case eStep.XY_MOVE_TO_X2Y2_CYL2_MOVING:
                //    IOLst.Y01 = true;
                //    Ipc.Motion.MoveAbs(2, 1000, X2_Pos, 500, 500);
                //    Ipc.Motion.MoveAbs(3, 1000, Y2_Pos, 500, 500);
                //    NextStep();
                //    break;
                //case eStep.XY_MOVE_TO_X2Y2_OK:
                //    if (Ipc.Motion.GetCurrentPos(2) == X2_Pos && Ipc.Motion.GetCurrentPos(3) == Y2_Pos&& !IOLst.X11)
                //    {
                //        NextStep();
                //    }
                //    break;
                //case eStep.Z_MOVE_TO_Z2:
                //    Ipc.Motion.MoveAbs(1, 1000, Z2_Pos, 500, 500);
                //    NextStep();
                //    break;
                //case eStep.Z_MOVE_TO_Z2_OK:
                //    if (Ipc.Motion.GetCurrentPos(1) == Z2_Pos)
                //    {
                //        NextStep();
                //    }
                //    break;
                //case eStep.VACUUM_OFF:
                //    IOLst.Y02 = false;
                //    NextStep();
                //    break;
                //case eStep.WAIT_X11_ON:
                //    if (IOLst.X11)
                //    {
                //        isPickProduct = false;
                //        if (Delay(300)) NextStep();
                //    }
                //    break;
                //case eStep.Z_MOVE_TO_Z1_WAIT_POS:
                //    Ipc.Motion.MoveAbs(1, 1000, Z1_Pos, 500, 500);
                //    NextStep();
                //    break;
                //case eStep.Z1_WAIT_POS_OK:
                //    if (Ipc.Motion.GetCurrentPos(1) == Z1_Pos)
                //    {
                //        NextStep();
                //    }
                //    break;
                //case eStep.CYL2_ORG:
                //    IOLst.Y01 = false;
                //    this.Stop();
                //    break;

            }
            
        }

    }
}
