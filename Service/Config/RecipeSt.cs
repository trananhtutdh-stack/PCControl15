using PCControl.Service.IPC;
using System.Reflection;

namespace PCControl.Service.Config
{
    public class RecipeSt
    {
        private static readonly PropertyInfo[] CylinderProperties = typeof(RecipeSt)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(CylinderSt))
            .OrderBy(p => int.Parse(p.Name[10..]))
            .ToArray();

        public string Id { get; set; } = "";

        public string Name { get; set; } = "";

        public CylinderSt CylinderSt1 { get; set; } = new();
        public CylinderSt CylinderSt2 { get; set; } = new();
        public CylinderSt CylinderSt3 { get; set; } = new();
        public CylinderSt CylinderSt4 { get; set; } = new();
        public CylinderSt CylinderSt5 { get; set; } = new();
        public CylinderSt CylinderSt6 { get; set; } = new();
        public CylinderSt CylinderSt7 { get; set; } = new();
        public CylinderSt CylinderSt8 { get; set; } = new();
        public CylinderSt CylinderSt9 { get; set; } = new();
        public CylinderSt CylinderSt10 { get; set; } = new();
        public CylinderSt CylinderSt11 { get; set; } = new();
        public CylinderSt CylinderSt12 { get; set; } = new();
        public CylinderSt CylinderSt13 { get; set; } = new();
        public CylinderSt CylinderSt14 { get; set; } = new();
        public CylinderSt CylinderSt15 { get; set; } = new();
        public CylinderSt CylinderSt16 { get; set; } = new();
        public CylinderSt CylinderSt17 { get; set; } = new();
        public CylinderSt CylinderSt18 { get; set; } = new();
        public CylinderSt CylinderSt19 { get; set; } = new();
        public CylinderSt CylinderSt20 { get; set; } = new();
        public CylinderSt CylinderSt21 { get; set; } = new();
        public CylinderSt CylinderSt22 { get; set; } = new();
        public CylinderSt CylinderSt23 { get; set; } = new();
        public CylinderSt CylinderSt24 { get; set; } = new();
        public CylinderSt CylinderSt25 { get; set; } = new();
        public CylinderSt CylinderSt26 { get; set; } = new();
        public CylinderSt CylinderSt27 { get; set; } = new();
        public CylinderSt CylinderSt28 { get; set; } = new();
        public CylinderSt CylinderSt29 { get; set; } = new();
        public CylinderSt CylinderSt30 { get; set; } = new();

        public PosSt[] PosData { get; set; } = Enumerable.Range(0, 4000).Select(_ => new PosSt()).ToArray();

        public PALLET TraySt { get; set; } = new();
        public PALLET JigCh1St { get; set; } = new();
        public PALLET JigCh2St { get; set; } = new();

        public void CopyFrom(RecipeSt source)
        {
            foreach (var prop in CylinderProperties)
                ((CylinderSt)prop.GetValue(this)!).CopyFrom((CylinderSt)prop.GetValue(source)!);
            PosData.Zip(source.PosData)
                   .ToList()
                   .ForEach(x => x.First.CopyFrom(x.Second));
            TraySt.CopyFrom(source.TraySt);
            JigCh1St.CopyFrom(source.JigCh1St);
            JigCh2St.CopyFrom(source.JigCh2St);
        }
    }
}
