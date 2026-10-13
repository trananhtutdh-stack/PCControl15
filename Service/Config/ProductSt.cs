using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PCControl.Service;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PCControl.Service.Config
{
    public class PalletSt
    {
        public PALLET TRAY {  get; set; } = new PALLET();
        public PALLET JIGCH1 { get; set; } = new PALLET();
        public PALLET JIGCH2 { get; set; } = new PALLET();
    }

    public partial class PALLET : ObservableObject
    {
        public PALLET()
        {
            this.Row = 5;
            this.Col = 4;
            this.PitchX = 1000;
            this.PitchY = 1000;
        }
        [ObservableProperty] private int row = 5;
        [ObservableProperty] private int col = 4;
        [ObservableProperty] private double pitchX = 1000;
        [ObservableProperty] private double pitchY = 1000;

        public PALLET Coppy()
        {
            PALLET pl = new PALLET();
            pl.Row = this.Row;
            pl.Col = this.Col;
            pl.PitchX = this.PitchX;
            pl.PitchY = this.PitchY;
            return pl;
        }

        public void CopyFrom(PALLET other)
        {
            Row = other.Row;
            Col = other.Col;
            PitchX = other.PitchX;
            PitchY = other.PitchY;
        }
    }
}
