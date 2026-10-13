using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCControl.Service.DeviceManager.Actuator
{
    public class ProductHand
    {
        private List<int> proLst;
        public int TargetTool
        {
            get
            {
                int idx = proLst.FindIndex(x => x == 1);
                return idx < 0 ? -1 : idx + 1;
            }
        }
        public ProductHand()
        {
            proLst = new List<int>() { 0, 0, 0, 0 };
        }
        public void PickUp(params int[] pro)
        {
            this.proLst = pro.ToList();
        }
        public void Remove(int idx)
        {
            if (idx > this.proLst.Count) return;
            proLst[idx - 1] = 0;
        }

        public IReadOnlyList<int> GetHeldStates() => proLst;
    }
}
