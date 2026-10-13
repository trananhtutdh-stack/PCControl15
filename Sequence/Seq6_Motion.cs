

namespace PCControl.Sequence
{
    public class Seq6_Motion : SeqBase
    {
        public enum eStep
        {
            IDLE = -1,
            SET_M28,
            GET_M28,
            SET_M29,
            GET_M29,
            SET_M30,
            GET_M30,
            SET_M31,
            GET_M31,
        }
        public Seq6_Motion(SeqThread seqTh) : base(seqTh)
        {


        }
        public override void Run()
        {
        }
    }
}
