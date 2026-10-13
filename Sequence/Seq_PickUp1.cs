

namespace PCControl.Sequence
{
    public class Seq_PickUp1 : SeqBase
    {
        public enum eStep
        {
            IDLE = -1,
            ROBOT_Z_UP1_SET = 0,
            ROBOT_Z_UP1_GET,
            ROBOT_MOVING_TO_TRAY_SET,
            ROBOT_MOVING_TO_TRAY_GET,
            ROBOT_Z_DOWN_SET,
            ROBOT_Z_DOWN_GET,
            VACCUM_SET,
            VACCUM_DELAY,
            ROBOT_Z_UP_REL_SET,
            ROBOT_Z_UP_REL_GET,
            VACCUM_GET,
            ROBOT_Z_UP2_SET,
            ROBOT_Z_UP2_GET,
            ROBOT_MOVING_TO_JIG_SET,
            ROBOT_MOVING_TO_JIG_GET
            ,
        }

        public Seq_PickUp1(SeqThread seqTh) : base(seqTh)
        {
        }
        public override void Run()
        {
            if (false == GetWork())
                return;
            switch ((eStep)Step)
            {
                
            }
        }
    }
}
