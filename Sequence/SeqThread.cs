using PCControl.Machine;
using PLCVir.Device;
using ShareLibrary.Logging;
using System.Threading;

namespace PCControl.Sequence
{
    public partial class SeqThread 
    {
        public readonly MachineDevices Devices;
        MyLogger log = new MyLogger("SeqThread");
        private const int ScanIntervalMs = 100;

        public Thread _seqThread;
        private CancellationTokenSource _cts;

        public Seq_LoadTray seq_LoadTray;
        public Seq_LoadJig1 seq_LoadJig1;
        public Seq_PickUp1 seq_PickUp1;
        public SeqStatus seq_Status;
        public SeqThread(MachineDevices devices)
        {
            Devices = devices;
            seq_LoadTray = new Seq_LoadTray(this);
            seq_LoadJig1 = new Seq_LoadJig1(this);
            seq_PickUp1 = new Seq_PickUp1(this);
            seq_Status = new SeqStatus(this);
            _cts = new CancellationTokenSource();
            var token = _cts.Token;

            _seqThread = new Thread(() => ProcessSeq(token))
            {
                IsBackground = true,
                Name = "SeqProcess",
            };
            _seqThread.Start();
        }
        private void ProcessSeq(CancellationToken token)
        {
            log.Create("Start Sequence Thread");
            //seq_LoadJig1.Start();
            seq_Status.Start();

            while (!token.IsCancellationRequested)
            {
                seq_LoadJig1.Run();
                seq_LoadTray.Run();
                seq_PickUp1.Run();
                seq_Status.Run();
            }
        }


        public void Stop()
        {
            if (_cts.IsCancellationRequested)
                return;

            _cts.Cancel();

            if (_seqThread.IsAlive &&
                Thread.CurrentThread != _seqThread)
            {
                _seqThread.Join(1000);
            }
        }

        public void Dispose()
        {
            Stop();
            _cts.Dispose();
        }

    }
}
