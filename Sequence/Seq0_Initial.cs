
namespace PCControl.Sequence
{
    public class Seq0_Initial : SeqBase
    {
        public enum eStep
        {
            IDLE = -1,
            WAIT_INITIAL = 0,
            SET_SCANNER_OPEN,
            GET_SCANNER_OPEN,
            SET_CYLINDER1_ORIGIN,
            GET_CYLINDER1_ORIGIN,
            SET_CYLINDER2_ORIGIN,
            GET_CYLINDER2_ORIGIN,
            SET_CYLINDER3_ORIGIN,
            GET_CYLINDER3_ORIGIN,
            INITIAL_NG,
            INITIAL_OK,
        }
        public Seq0_Initial(SeqThread seqTh) : base(seqTh)
        {
        }
        public override void Run()
        {

        }
    }
}
