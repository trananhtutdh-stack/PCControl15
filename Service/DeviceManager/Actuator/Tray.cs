using PCControl.Service.IPC;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using PCControl.Service.Config;
using AutoVision.Service.IPC;

namespace PCControl.Service.DeviceManager.Actuator
{
    public partial class TrayItem : ObservableObject
    {
        [ObservableProperty] private int index;
        [ObservableProperty] private bool status;
        [ObservableProperty] private bool isTarget;
    }

    public partial class TRAY : ObservableObject
    {
        [ObservableProperty] private PALLET pallet;
        [ObservableProperty] private int tool;
        [ObservableProperty] private int targetPos;
        private readonly IIoPoint trayDectSS;
        public bool IsEmpty { get; private set; }
        public bool IsTrayDetect => trayDectSS.Value;
        public List<PosData> PosDatasCh1 { get; } = new();
        public List<PosData> PosDatasCh2 { get; } = new();
        public Dictionary<int, PosIndex> Tray_Index { get; private set; } = new();
        public ObservableCollection<TrayItem> Tray_Status { get; private set; } = new();
        private int GroupCount => Tool > 0 ? Tray_Status.Count / Tool : 0;
        public TRAY(PALLET pallet, IIoPoint trayDectSS, int tool)
        {
            Pallet = pallet;
            this.trayDectSS = trayDectSS;
            Tool = tool;
            TargetPos = 0;
            RebuildIndex();
            RebuildStatus(true);
            IsEmpty = false;
            pallet.PropertyChanged += Pallet_PropertyChanged;
        }
        partial void OnTargetPosChanged(int value) => ApplyTargetHighlight(value);
        private void Pallet_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            UpdateTray();
        }
        private (int start, int end) GetGroupRange(int groupIndex)
        {
            int start = (groupIndex - 1) * Tool + 1;
            int end = start + Tool - 1;
            return (start, end);
        }
        private void ApplyTargetHighlight(int groupIndex)
        {
            if (Tray_Status == null)
                return;
            foreach (var tray in Tray_Status)
                tray.IsTarget = false;
            if (groupIndex < 1 || groupIndex > GroupCount)
                return;
            var (start, end) = GetGroupRange(groupIndex);
            foreach (var tray in Tray_Status)
            {
                if (tray.Index >= start && tray.Index <= end)
                    tray.IsTarget = true;
            }
        }
        private void RebuildIndex()
        {
            Tray_Index = Enumerable.Range(1, Pallet.Col * Pallet.Row)
                .ToDictionary(
                    x => x,
                    x => new PosIndex(
                        (x - 1) / Pallet.Col,
                        (x - 1) % Pallet.Col));
        }
        private void RebuildStatus(bool allPresent)
        {
            Tray_Status = new ObservableCollection<TrayItem>(
                Enumerable.Range(1, Pallet.Col * Pallet.Row)
                    .Select(x => new TrayItem
                    {
                        Index = x,
                        Status = allPresent
                    }));
            OnPropertyChanged(nameof(Tray_Status));
        }
        public void RemoveProductPos(int groupIndex)
        {
            if (Tray_Status == null || groupIndex < 1 || groupIndex > GroupCount)
                return;

            var (start, end) = GetGroupRange(groupIndex);
            foreach (var tray in Tray_Status)
            {
                if (tray.Index >= start && tray.Index <= end)
                {
                    tray.IsTarget = false;
                    tray.Status = false;
                }
                    
            }
            IsEmpty = Tray_Status.All(x => !x.Status);
        }
        public void UpdateTray()
        {
            int savedGroup = TargetPos;
            RebuildIndex();
            RebuildStatus(true);
            IsEmpty = false;
            OnPropertyChanged(nameof(Tray_Index));
            TargetPos = 0;
            if (savedGroup > 0)
                TargetPos = savedGroup;
        }
        public void ClearData()
        {
            foreach (var tray in Tray_Status)
                tray.Status = true;
            IsEmpty = false;
        }
        public void SelectGroup(int cellIndex)
        {
            if (Tray_Status == null || cellIndex < 1 || cellIndex > Tray_Status.Count)
                return;

            TargetPos = (cellIndex - 1) / Tool + 1;
        }
        public void CreatePos(double posX1st, double posY1st, double posXSpeed, double posYSpeed, int ch)
        {
            var posDatas = ch == 1 ? PosDatasCh1 : PosDatasCh2;
            posDatas.Clear();

            int groupCols = Tool > 0 ? Pallet.Col / Tool : 0;
            for (int row = 0; row < Pallet.Row; row++)
            {
                for (int col = 0; col < groupCols; col++)
                {
                    double x = posX1st + col * Pallet.PitchX;
                    double y = posY1st + row * Pallet.PitchY;
                    posDatas.Add(new PosData(x, y, posXSpeed, posYSpeed));
                }
            }
        }
        public PosData FindPos(int groupIndex, int ch)
        {
            
            var posDatas = ch == 1 ? PosDatasCh1 : PosDatasCh2;
            return posDatas[groupIndex - 1];
        }
        public void StatusJugger()
        {
            OnPropertyChanged(nameof(IsTrayDetect));
        }
    }
}
