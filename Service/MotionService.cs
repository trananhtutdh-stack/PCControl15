using AutoVision.Service.IPC;

namespace PCControl.Service
{
    public sealed class MotionService
    {
        public void ServoOn(IAxis axis)
        {
            axis.ServoOn();
        }

        public void ServoOff(IAxis axis)
        {
            axis.ServoOff();
        }

        public void JogPositive(IAxis axis)
        {
            axis.JOG_Positive();
        }

        public void JogNegative(IAxis axis)
        {
            axis.JOG_Nagative();
        }

        public void JogStop(IAxis axis)
        {
            axis.JOG_STOP();
        }

        public void StopAxis(IAxis axis)
        {
            axis.JOG_STOP();
        }

        public void Home(IAxis axis)
        {
            axis.Homing(axis.HomeSpeed,axis.CreepSpeed);
        }

        public void ClearError(IAxis axis)
        {
            axis.AxisErrClear();
        }

        public void MoveAbsolute(IAxis axis,double target,double speed)
        {
            axis.MoveAbs(target,speed);
        }

        public void MoveRelative(IAxis axis,double distance,double speed)
        {
            axis.MoveRel(distance,speed);
        }
    }
}