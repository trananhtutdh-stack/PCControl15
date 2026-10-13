using PCControl.Service.IPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using AutoVision.Service.IPC;

namespace PCControl.Service
{
    public class Tool : ObservableObject
    {
        private string name;
        private IIoPoint vaccum;
        private IIoPoint blow;
        private IIoPoint vaccumSS;
        private IIoPoint blowSS;
        private int index;

        private bool isVaccumOld = false, isBlowOld = false;

        public string Name => name;
        public bool IsVaccumSS => vaccumSS.Value;
        public bool IsBlowSS => blowSS.Value;
        public bool IsVaccum => vaccum.Value;
        public bool IsBlow => blow.Value;
        public int Index => index;

        public void Vaccum()
        {
            blow.Value = false;
            vaccum.Value = true;
            OnPropertyChanged(nameof(IsVaccum));
            OnPropertyChanged(nameof(IsBlow));
        }
        public void Blow()
        {
            vaccum.Value = false;
            blow.Value = true;
            OnPropertyChanged(nameof(IsVaccum));
            OnPropertyChanged(nameof(IsBlow));
        }
        public void Off()
        {
            vaccum.Value = false;
            blow.Value = false;
            OnPropertyChanged(nameof(IsVaccum));
            OnPropertyChanged(nameof(IsBlow));
        }
        public Tool(IIoPoint vaccumSS, IIoPoint blowSS,IIoPoint vaccum, IIoPoint blow, string name, int idx)
        {
            this.vaccum = vaccum;
            this.blow = blow;
            this.vaccumSS = vaccumSS;
            this.blowSS = blowSS;
            this.name = name;
            this.index = idx;
        }

        public void StatusJugger()
        {
            if (vaccumSS.Value != isVaccumOld)
            {
                OnPropertyChanged(nameof(IsVaccumSS));
                isVaccumOld = vaccumSS.Value;
            }
            if (blowSS.Value != isBlowOld)
            {
                OnPropertyChanged(nameof(IsBlowSS));
                isBlowOld = blowSS.Value;
            }
        }
    }
}
