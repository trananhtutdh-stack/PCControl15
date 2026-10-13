using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PCControl.Core.IPC;

namespace PCControl.Service.Config
{
    public class RecipeSetting
    {
        private static readonly PropertyInfo[] CylinderProperties = typeof(RecipeSetting)
            .GetProperties()
            .Where(p => p.PropertyType == typeof(Cylinder))
            .OrderBy(p => int.Parse(p.Name[10..]))
            .ToArray();
        public string Id { get; set; } = "";
        public string Name { get; set; } = "";

        ////cylinder setting
        public CylinderSetting CylinderSt1 { get; set; } = new();
        public CylinderSetting CylinderSt2 { get; set; } = new();
        public CylinderSetting CylinderSt3 { get; set; } = new();
        public CylinderSetting CylinderSt4 { get; set; } = new();
        public CylinderSetting CylinderSt5 { get; set; } = new();
        public CylinderSetting CylinderSt6 { get; set; } = new();



        public PosSetting[] PosData { get; set; } = Enumerable.Range(0,2800).Select(_=> new PosSetting()).ToArray();

        public PANEL TraySetting { get; set; } = new();
        public PANEL JigCh1Setting { get; set; } = new();
        public PANEL JigCh2Setting { get; set; } = new();

        public void CopyFrom(RecipeSetting source)
        {
            foreach (var property in CylinderProperties)
                ((CylinderSetting)property.GetValue(this)!).CopyFrom((CylinderSetting)property.GetValue(source)!);
            PosData.Zip(source.PosData)
                .ToList()
                .ForEach(x => x.First.CopyFrom(x.Second));
            TraySetting.CopyFrom(source.TraySetting);
            JigCh1Setting.CopyFrom(source.JigCh1Setting);
            JigCh2Setting.CopyFrom(source.JigCh2Setting);
        }
    }

    public class CylinderSetting
    {
        public int OriginTime { get; set; } = 100;
        public int MoveTime { get; set; } = 100;
        public int AlarmTime { get; set; } = 100;
        public bool NoMoveShield { get; set; }
        public bool NoOriginShield { get; set; }
        public void CopyFrom(CylinderSetting source)
        {
            source.OriginTime = OriginTime;
            source.MoveTime = MoveTime;
            source.AlarmTime = AlarmTime;
            source.NoMoveShield = NoMoveShield;
            source.NoOriginShield = NoOriginShield;
        }
    }
}
