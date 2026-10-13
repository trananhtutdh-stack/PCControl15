
using System.Windows;
using AutoVision.Service.IPC;
using PCControl.Machine;

namespace PCControl.Sequence
{
    public class Seq_LoadJig1 : SeqBase
    {
        
        public enum eStep
        {
            IDLE = 0,
            JIG_DETECT,
            CYL1_MOVING,
            Z_MOVE_TO_Z1,
            Z_MOVE_TO_Z1_OK,
            XY_MOVE_TO_X1Y1,
            XY_MOVE_TO_X1Y1_OK,
            Z_MOVE_TO_Z2,
            Z_MOVE_TO_Z2_OK,
            VACUUM_ON,
            Z_MOVE_TO_Z1_2,
            Z_MOVE_TO_Z1_2_OK,
            CYL1_ORG
        }

        public Seq_LoadJig1(SeqThread seqTh) : base(seqTh)
        {
            
        }

        public override void Run()
        {
            if (false == GetWork())
                return;
            switch ((eStep)Step)
            {
                //case eStep.IDLE:
                    
                //    Ipc.Motion.ServoOn(1);
                //    Ipc.Motion.ServoOn(2);
                //    Ipc.Motion.ServoOn(3);
                //    Ipc.Motion.Homing(1000, 200, 1);
                //    Ipc.Motion.Homing(1000, 200, 2);
                //    Ipc.Motion.Homing(1000, 200, 3);
                //    IOLst.Y00 = false;
                //    IOLst.Y01 = false;
                //    IOLst.Y02 = false;
                //    isPickProduct = false;
                //    NextStep();
                //    break;
                //case eStep.JIG_DETECT:
                //    if (IOLst.X10)
                //    {
                //        isPickProduct = false;
                //        if (Delay(2000)) NextStep();
                //    }
                //    break;
                //case eStep.CYL1_MOVING:
                //    IOLst.Y00 = true;
                //    if (Delay(2000)) NextStep();
                //    break;
                //case eStep.Z_MOVE_TO_Z1:
                //    Ipc.Motion.MoveAbs(1, 1000, Z1_Pos, 500, 500);
                //    NextStep();
                //    break;
                //case eStep.Z_MOVE_TO_Z1_OK:
                //    if (Ipc.Motion.GetCurrentPos(1) == Z1_Pos && isPickProduct == false)
                //    {
                //        NextStep();
                //    }
                //    if (Ipc.Motion.GetCurrentPos(1) == Z1_Pos && isPickProduct == true)
                //    {
                //        MoveStep((int)eStep.CYL1_ORG);
                //    }
                //    break;
                //case eStep.XY_MOVE_TO_X1Y1:
                //    Ipc.Motion.MoveAbs(2, 1000, X1_Pos, 500, 500);
                //    Ipc.Motion.MoveAbs(3, 1000, Y1_Pos, 500, 500);
                //    NextStep();
                //    break;
                //case eStep.XY_MOVE_TO_X1Y1_OK:
                //    if (Ipc.Motion.GetCurrentPos(2) == X1_Pos && Ipc.Motion.GetCurrentPos(3) == Y1_Pos)
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
                //case eStep.VACUUM_ON:
                //    IOLst.Y02 = true;
                //    isPickProduct = true;
                //    if (Delay(2000)) MoveStep((int)eStep.Z_MOVE_TO_Z1);
                //    break;
                //case eStep.CYL1_ORG:
                //    IOLst.Y00 = false;
                //    SeqTh.seq_LoadTray.Start();
                //    if (Delay(2000)) MoveStep((int)eStep.JIG_DETECT);
                //    break;

            }
        }
    }
}
