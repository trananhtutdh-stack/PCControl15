
using System.Diagnostics;
using System.Windows;
using AutoVision.Service.IPC;
using PCControl.Machine;

namespace PCControl.Sequence
{
    public abstract class SeqBase
    {
        protected readonly SeqThread SeqTh;

        private bool _work;
        public int Step { get; set; }

        public Stopwatch _timer = new Stopwatch();
        
        public bool isPickProduct { get; set; } = false;
        public bool isEndCycle;
        
        protected SeqBase(SeqThread seqTh)
        {
            SeqTh = seqTh;
            _work = false;
            Step = 0;
            
        }

        public abstract void Run();

        public bool GetWork() => _work;

        public void Start()
        {
            _work = true;
            Step = 0;
            
        }

        public virtual void Stop()
        {
            _work = false;
            Step = -1;
        }

        public void PreStep()
        {
            Step--;
            _timer.Reset();
        }

        public void NextStep()
        {
            Step++;
            _timer.Reset();
        }

        public void MoveStep(int step)
        {
            Step = step;
            _timer.Reset();
        }

        public bool Delay(double setTime)
        {
            if (_timer.IsRunning == false)
                _timer.Start();
            return setTime < _timer.ElapsedMilliseconds;
        }
    }
    
    
}
