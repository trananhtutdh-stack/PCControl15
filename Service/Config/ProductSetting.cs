using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace PCControl.Service.Config
{
    public class ProductSetting
    {

    }
    public class PanelSetting 
    {
        public PANEL TRAY {  get; set; } = new PANEL();
        public PANEL JIGCH1 { get; set; } = new PANEL();
        public PANEL JIGCH2 { get; set; } = new PANEL();
    }
    public partial class PANEL : ObservableObject
    {
        [ObservableProperty]
        private int row = 5;
        [ObservableProperty]
        private int col = 4;
        [ObservableProperty]
        private double pitchX = 1000;
        [ObservableProperty]
        private double pitchY = 1000;
        public PANEL()
        {
            this.row = 5;
            this.col = 4;
            this.pitchX = 1000;
            this.pitchY = 1000;
        }

        public PANEL Coppy()
        {
            PANEL pl = new PANEL();
            pl.Row = this.Row;
            pl.Col = this.Col;
            pl.PitchX = this.PitchX;
            pl.PitchY = this.PitchY;
            return pl;
        }
        public void CopyFrom(PANEL other)
        {
            Row = other.Row;
            Col = other.Col;
            PitchX = other.PitchX;
            PitchY = other.PitchY;
        }
    }
}
