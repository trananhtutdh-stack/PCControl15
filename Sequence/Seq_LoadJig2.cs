

namespace PCControl.Sequence
{
    public class Seq1_Load : SeqBase
    {
        public enum eStep
        {
            IDLE = -1,
            JIG_DETECT,
            WAIT_START,
            JIG_MOVING,
            JIG_MOVING_CHECK,
            ROBOT_PICKUP,
        }
        public Seq1_Load(SeqThread seqTh) : base(seqTh)
        {
        }
        public override void Run()
        {
        }

    }
}
