using AutoVision.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using PCControl.Service.Config;
using PCControl.Service.IPC;
using System.Collections.ObjectModel;

namespace PCControl.Service.DeviceManager.Actuator
{
    public partial class JigItem : ObservableObject
    {
        [ObservableProperty]
        private int index;

        [ObservableProperty]
        private bool status;

        [ObservableProperty]
        private bool isTarget;
    }
    public partial class JIG : ObservableObject
    {
        [ObservableProperty]
        private PALLET pallet;

        [ObservableProperty]
        private int targetPos;


        private IIoPoint jigDectSS;

        public Dictionary<int, PosIndex> Jig_Index { get; private set; }

        public ObservableCollection<JigItem> Jig_Status { get; private set; }

        public List<PosData>[] PosDatas { get; private set; } = new List<PosData>[4];

        public int CurrentPos { get; private set; }

        public bool IsJigDetect => jigDectSS.Value;

        public bool IsFull { get; private set; }

        public JIG(PALLET pallet, IIoPoint jigDectSS)
        {
            Pallet = pallet;
            this.jigDectSS = jigDectSS;

            Jig_Index =
                Enumerable.Range(1, pallet.Col * pallet.Row)
                .ToDictionary(
                    x => x,
                    x => new PosIndex(
                        (x - 1) / pallet.Col,
                        (x - 1) % pallet.Col));

            Jig_Status =
                new ObservableCollection<JigItem>(
                    Enumerable.Range(1, pallet.Col * pallet.Row)
                    .Select(x => new JigItem
                    {
                        Index = x,
                        Status = false
                    }));
            for (int i = 0; i < PosDatas.Length; i++)
            {
                PosDatas[i] = new List<PosData>();
            }
            TargetPos = 0;
            Pallet.PropertyChanged += Pallet_PropertyChanged;
        }
        partial void OnTargetPosChanged(int value)
        {
            foreach (var jig in Jig_Status)
            {
                jig.IsTarget = false;
            }

            Jig_Status[value - 1].IsTarget = true;
        }
        private void Pallet_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateJig();
        }
        public void RemoveEmptyPos(int pos)
        {
            if (pos > Jig_Status.Count)
                return;

            Jig_Status[pos - 1].Status = true;

            IsFull = Jig_Status.All(x => x.Status);
            if(!IsFull) TargetPos = pos + 1;
        }

        public void UpdateJig()
        {
            Jig_Index =
                Enumerable.Range(1, Pallet.Col * Pallet.Row)
                .ToDictionary(
                    x => x,
                    x => new PosIndex(
                        (x - 1) / Pallet.Col,
                        (x - 1) % Pallet.Col));

            Jig_Status?.Clear();

            foreach (int i in Enumerable.Range(1, Pallet.Col * Pallet.Row))
            {
                Jig_Status?.Add(new JigItem
                {
                    Index = i,
                    Status = false
                });
            }

            OnPropertyChanged(nameof(Jig_Index));
        }

        public void ClearData()
        {
            Jig_Status.ToList().ForEach(x => x.Status = false);
            TargetPos = 0;
        }

        public void SelectJigPos(int pos)
        {
            foreach (var jig in Jig_Status)
            {
                jig.IsTarget = false;
            }

            Jig_Status[pos - 1].IsTarget = true;

            TargetPos = pos;
        }

        public void CreatePos(double posX1st,double posY1st, double posXSpeed,double posYSpeed, int tool)
        {
            PosDatas[tool].Clear();
            for (int row = 0; row < Pallet.Row; row++)
            {
                for (int col = 0; col < Pallet.Col; col++)
                {
                    double x = posX1st + (col * Pallet.PitchX);

                    double y = posY1st + (row * Pallet.PitchY);
                    PosDatas[tool].Add(new PosData(x,y,posXSpeed,posYSpeed));
                }
            }
        }
        public PosData FindPos(int pos, int tool)
        {
            return PosDatas[tool -1][pos - 1];
        }
        public void StatusJugger()
        {
            OnPropertyChanged(nameof(IsJigDetect));
        }
    }

    public record PosIndex(int Row, int Column);

    public record PosData(double XPos,double YPos,double XSpeed,double YSpeed);
}