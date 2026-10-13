using System;
using AutoVision.Service.IPC;
using PCControl.Service;
using PCControl.Service.Config;
using PCControl.Service.DeviceManager.Actuator;
using PCControl.Service.IPC;

namespace PCControl.Machine
{
    public sealed class MachineDevices : IDisposable
    {
        public IIpc Ipc { get; }
        public Dictionary<string, Cylinder1> Cylinders { get; } = new();
        public Dictionary<string, IAxis> Axis { get; } = new();

        public Dictionary<string, Tool> Tools { get; } = new();



        public IOLst IOLst { get; }

        public SingleCylinder CYL_JIG_CH1 { get; }
        public SingleCylinder CYL_JIG_CH2 { get; }
        public SingleCylinder CYL_TRAY { get; }
        public DoubleCylinder CYL_TOOL1_CH1 { get; }
        public DoubleCylinder CYL_TOOL2_CH1 { get; }
        public DoubleCylinder CYL_TOOL3_CH1 { get; }
        public DoubleCylinder CYL_TOOL4_CH1 { get; }

        public IAxis CH1_X_AXIS { get; }
        public IAxis CH1_Y_AXIS { get; }
        public IAxis CH1_Z_AXIS { get; }
        public IAxis CH2_X_AXIS { get; }
        public IAxis CH2_Y_AXIS { get; }
        public IAxis CH2_Z_AXIS { get; }

        public Tool TOOL1_CH1 { get; }
        public Tool TOOL2_CH1 { get; }
        public Tool TOOL3_CH1 { get; }
        public Tool TOOL4_CH1 { get; }
        public ProductHand ProductHandCh1 { get; }
        public JIG JigCh1 { get; }
        public JIG JigCh2 { get; }
        public TRAY Tray { get; }










        public MachineDevices(IIpcFactory ipcFactory)
        {
            Ipc = ipcFactory.Create(MAKER.WMX);

            //Cylinders.Add("CYL_JIG_CH1", new Cylinder("CYL_JIG_CH1", "Y0", "Y1", "X0", "X1", Ipc));
            //Cylinders.Add("CYL_JIG_CH2", new Cylinder("CYL_JIG_CH2", "Y2", "Y3", "X2", "X3", Ipc));
            //Cylinders.Add("CYL_TRAY", new Cylinder("CYL_TRAY", "Y4", "Y5", "X4", "X5", Ipc));
            //Cylinders.Add("CYL_TOOL1_CH1", new Cylinder("CYL_TOOL1_CH1", "Y6", "Y7", "X6", "X7", Ipc));
            //Cylinders.Add("CYL_TOOL2_CH1", new Cylinder("CYL_TOOL2_CH1", "Y10", "Y11", "X10", "X11", Ipc));
            //Cylinders.Add("CYL_TOOL3_CH1", new Cylinder("CYL_TOOL3_CH1", "Y12", "Y13", "X12", "X13", Ipc));
            //Cylinders.Add("CYL_TOOL4_CH1", new Cylinder("CYL_TOOL4_CH1", "Y14", "Y15", "X14", "X15", Ipc));

            AddAxis("CH1_X_AXIS", 1);
            AddAxis("CH1_Y_AXIS", 2);
            AddAxis("CH1_Z_AXIS", 3);
            AddAxis("CH2_X_AXIS", 4);
            AddAxis("CH2_Y_AXIS", 5);
            AddAxis("CH2_Z_AXIS", 6);

            IOLst = new IOLst(Ipc);

            CYL_JIG_CH1 = new SingleCylinder(IOLst.X3, IOLst.X4, IOLst.Y2, "Jig#CH1", new CylinderSt
            {
                OriginTime = 150,
                MoveTime = 150,
                AlarmTime = 3000,
                NoMoveShield = true,
                NoOriginShield = true
            });
            CYL_JIG_CH2 = new SingleCylinder(IOLst.X5, IOLst.X6, IOLst.Y3, "Jig#CH2", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });
            CYL_TRAY = new SingleCylinder(IOLst.X7, IOLst.X10, IOLst.Y4, "Tray", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });
            CYL_TOOL1_CH1 = new DoubleCylinder(IOLst.X16, IOLst.X17, IOLst.Y16, IOLst.Y17, "CH1_TOOL#1", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });
            CYL_TOOL2_CH1 = new DoubleCylinder(IOLst.X20, IOLst.X21, IOLst.Y20, IOLst.Y21, "CH1_TOOL#2", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });
            CYL_TOOL3_CH1 = new DoubleCylinder(IOLst.X22, IOLst.X23, IOLst.Y22, IOLst.Y23, "CH1_TOOL#3", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });
            CYL_TOOL4_CH1 = new DoubleCylinder(IOLst.X24, IOLst.X25, IOLst.Y24, IOLst.Y25, "CH1_TOOL#4", new CylinderSt
            {
                OriginTime = 200,
                MoveTime = 200,
                AlarmTime = 3000,
                NoMoveShield = false,
                NoOriginShield = false
            });

            CH1_X_AXIS = Ipc.Motion.CreateAxis(0);
            CH1_X_AXIS.Name = "CH1_X Axis";

            CH1_Y_AXIS = Ipc.Motion.CreateAxis(1);
            CH1_Y_AXIS.Name = "CH1_Y Axis";

            CH1_Z_AXIS = Ipc.Motion.CreateAxis(1);
            CH1_Z_AXIS.Name = "CH1_Z Axis";

            CH2_X_AXIS = Ipc.Motion.CreateAxis(0);
            CH2_X_AXIS.Name = "CH2_X Axis";

            CH2_Y_AXIS = Ipc.Motion.CreateAxis(1);
            CH2_Y_AXIS.Name = "CH2_Y Axis";

            CH2_Z_AXIS = Ipc.Motion.CreateAxis(1);
            CH2_Z_AXIS.Name = "CH2_Z Axis";

            TOOL1_CH1 = new Tool(IOLst.X26, IOLst.X32, IOLst.Y26, IOLst.Y32, "Tool#1", 1);
            TOOL2_CH1 = new Tool(IOLst.X27, IOLst.X33, IOLst.Y27, IOLst.Y33, "Tool#2", 2);
            TOOL3_CH1 = new Tool(IOLst.X30, IOLst.X34, IOLst.Y30, IOLst.Y34, "Tool#3", 3);
            TOOL4_CH1 = new Tool(IOLst.X31, IOLst.X35, IOLst.Y31, IOLst.Y35, "Tool#4", 4);
            ProductHandCh1 = new ProductHand();

            JigCh1 = new JIG(new PALLET(), IOLst.X7);
            JigCh2 = new JIG(new PALLET(), IOLst.X47);
            Tray = new TRAY(new PALLET(), IOLst.X6, 4);

        }

        private void AddAxis(string name,int axisNo)
        {
            IAxis axis =Ipc.Motion.CreateAxis(axisNo);
            axis.Name = name;
            Axis.Add(name, axis);
        }

        public void Dispose()
        {
            Ipc?.Dispose();
        }
    }
}
