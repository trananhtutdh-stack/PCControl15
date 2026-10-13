using AutoVision.Service.IPC;
using PCControl.Service.IPC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCControl.Machine
{
    public class IOLst
    {
        readonly IIpc Ipc;
        public Dictionary<string, string> IOMap = new Dictionary<string, string>();

        public IOLst(IIpc ipc)
        {
            Ipc = ipc;
            LoadIOMap();
        }

        private void LoadIOMap()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "DATA", "IO.csv");
            if (!File.Exists(path))
                return;

            string[] lines = File.ReadAllLines(path);

            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                var parts = line.Split(',');

                if (parts.Length >= 1 && !string.IsNullOrWhiteSpace(parts[0]))
                {
                    var key = parts[0].Trim();
                    var value = parts.Length >= 2 ? parts[1].Trim() : "";

                    IOMap.TryAdd(key, value);
                }

                if (parts.Length >= 3 && !string.IsNullOrWhiteSpace(parts[2]))
                {
                    var key = parts[2].Trim();
                    var value = parts.Length >= 4 ? parts[3].Trim() : "";

                    IOMap.TryAdd(key, value);
                }
            }
        }

        private readonly Dictionary<(int slave, int bit, IoModes mode), IIoPoint> _ioCache
                = new Dictionary<(int, int, IoModes), IIoPoint>();

        private IIoPoint CreateIOPoint(int slave, int bit, IoModes ioMode, string adress)
        {
            var key = (slave, bit, ioMode);
            if (_ioCache.TryGetValue(key, out var io))
                return io;

            io = new WMX_IOPoint((WMX)Ipc, slave, bit, ioMode, adress);
            _ioCache[key] = io;
            return io;
        }

        public IIoPoint X0 => CreateIOPoint(0, 0, IoModes.input, nameof(X0));
        public IIoPoint X1 => CreateIOPoint(0, 1, IoModes.input, nameof(X1));
        public IIoPoint X2 => CreateIOPoint(0, 2, IoModes.input, nameof(X2));
        public IIoPoint X3 => CreateIOPoint(0, 3, IoModes.input, nameof(X3));
        public IIoPoint X4 => CreateIOPoint(0, 4, IoModes.input, nameof(X4));
        public IIoPoint X5 => CreateIOPoint(0, 5, IoModes.input, nameof(X5));
        public IIoPoint X6 => CreateIOPoint(0, 6, IoModes.input, nameof(X6));
        public IIoPoint X7 => CreateIOPoint(0, 7, IoModes.input, nameof(X7));

        public IIoPoint X10 => CreateIOPoint(1, 0, IoModes.input, nameof(X10));
        public IIoPoint X11 => CreateIOPoint(1, 1, IoModes.input, nameof(X11));
        public IIoPoint X12 => CreateIOPoint(1, 2, IoModes.input, nameof(X12));
        public IIoPoint X13 => CreateIOPoint(1, 3, IoModes.input, nameof(X13));
        public IIoPoint X14 => CreateIOPoint(1, 4, IoModes.input, nameof(X14));
        public IIoPoint X15 => CreateIOPoint(1, 5, IoModes.input, nameof(X15));
        public IIoPoint X16 => CreateIOPoint(1, 6, IoModes.input, nameof(X16));
        public IIoPoint X17 => CreateIOPoint(1, 7, IoModes.input, nameof(X17));

        public IIoPoint X20 => CreateIOPoint(2, 0, IoModes.input, nameof(X20));
        public IIoPoint X21 => CreateIOPoint(2, 1, IoModes.input, nameof(X21));
        public IIoPoint X22 => CreateIOPoint(2, 2, IoModes.input, nameof(X22));
        public IIoPoint X23 => CreateIOPoint(2, 3, IoModes.input, nameof(X23));
        public IIoPoint X24 => CreateIOPoint(2, 4, IoModes.input, nameof(X24));
        public IIoPoint X25 => CreateIOPoint(2, 5, IoModes.input, nameof(X25));
        public IIoPoint X26 => CreateIOPoint(2, 6, IoModes.input, nameof(X26));
        public IIoPoint X27 => CreateIOPoint(2, 7, IoModes.input, nameof(X27));

        public IIoPoint X30 => CreateIOPoint(3, 0, IoModes.input, nameof(X30));
        public IIoPoint X31 => CreateIOPoint(3, 1, IoModes.input, nameof(X31));
        public IIoPoint X32 => CreateIOPoint(3, 2, IoModes.input, nameof(X32));
        public IIoPoint X33 => CreateIOPoint(3, 3, IoModes.input, nameof(X33));
        public IIoPoint X34 => CreateIOPoint(3, 4, IoModes.input, nameof(X34));
        public IIoPoint X35 => CreateIOPoint(3, 5, IoModes.input, nameof(X35));
        public IIoPoint X36 => CreateIOPoint(3, 6, IoModes.input, nameof(X36));
        public IIoPoint X37 => CreateIOPoint(3, 7, IoModes.input, nameof(X37));

        public IIoPoint X40 => CreateIOPoint(4, 0, IoModes.input, nameof(X40));
        public IIoPoint X41 => CreateIOPoint(4, 1, IoModes.input, nameof(X41));
        public IIoPoint X42 => CreateIOPoint(4, 2, IoModes.input, nameof(X42));
        public IIoPoint X43 => CreateIOPoint(4, 3, IoModes.input, nameof(X43));
        public IIoPoint X44 => CreateIOPoint(4, 4, IoModes.input, nameof(X44));
        public IIoPoint X45 => CreateIOPoint(4, 5, IoModes.input, nameof(X45));
        public IIoPoint X46 => CreateIOPoint(4, 6, IoModes.input, nameof(X46));
        public IIoPoint X47 => CreateIOPoint(4, 7, IoModes.input, nameof(X47));

        public IIoPoint X50 => CreateIOPoint(5, 0, IoModes.input, nameof(X50));
        public IIoPoint X51 => CreateIOPoint(5, 1, IoModes.input, nameof(X51));
        public IIoPoint X52 => CreateIOPoint(5, 2, IoModes.input, nameof(X52));
        public IIoPoint X53 => CreateIOPoint(5, 3, IoModes.input, nameof(X53));
        public IIoPoint X54 => CreateIOPoint(5, 4, IoModes.input, nameof(X54));
        public IIoPoint X55 => CreateIOPoint(5, 5, IoModes.input, nameof(X55));
        public IIoPoint X56 => CreateIOPoint(5, 6, IoModes.input, nameof(X56));
        public IIoPoint X57 => CreateIOPoint(5, 7, IoModes.input, nameof(X57));

        public IIoPoint X60 => CreateIOPoint(6, 0, IoModes.input, nameof(X60));
        public IIoPoint X61 => CreateIOPoint(6, 1, IoModes.input, nameof(X61));
        public IIoPoint X62 => CreateIOPoint(6, 2, IoModes.input, nameof(X62));
        public IIoPoint X63 => CreateIOPoint(6, 3, IoModes.input, nameof(X63));
        public IIoPoint X64 => CreateIOPoint(6, 4, IoModes.input, nameof(X64));
        public IIoPoint X65 => CreateIOPoint(6, 5, IoModes.input, nameof(X65));
        public IIoPoint X66 => CreateIOPoint(6, 6, IoModes.input, nameof(X66));
        public IIoPoint X67 => CreateIOPoint(6, 7, IoModes.input, nameof(X67));

        public IIoPoint Y0 => CreateIOPoint(0, 0, IoModes.output, nameof(Y0));
        public IIoPoint Y1 => CreateIOPoint(0, 1, IoModes.output, nameof(Y1));
        public IIoPoint Y2 => CreateIOPoint(0, 2, IoModes.output, nameof(Y2));
        public IIoPoint Y3 => CreateIOPoint(0, 3, IoModes.output, nameof(Y3));
        public IIoPoint Y4 => CreateIOPoint(0, 4, IoModes.output, nameof(Y4));
        public IIoPoint Y5 => CreateIOPoint(0, 5, IoModes.output, nameof(Y5));
        public IIoPoint Y6 => CreateIOPoint(0, 6, IoModes.output, nameof(Y6));
        public IIoPoint Y7 => CreateIOPoint(0, 7, IoModes.output, nameof(Y7));

        public IIoPoint Y10 => CreateIOPoint(1, 0, IoModes.output, nameof(Y10));
        public IIoPoint Y11 => CreateIOPoint(1, 1, IoModes.output, nameof(Y11));
        public IIoPoint Y12 => CreateIOPoint(1, 2, IoModes.output, nameof(Y12));
        public IIoPoint Y13 => CreateIOPoint(1, 3, IoModes.output, nameof(Y13));
        public IIoPoint Y14 => CreateIOPoint(1, 4, IoModes.output, nameof(Y14));
        public IIoPoint Y15 => CreateIOPoint(1, 5, IoModes.output, nameof(Y15));
        public IIoPoint Y16 => CreateIOPoint(1, 6, IoModes.output, nameof(Y16));
        public IIoPoint Y17 => CreateIOPoint(1, 7, IoModes.output, nameof(Y17));

        public IIoPoint Y20 => CreateIOPoint(2, 0, IoModes.output, nameof(Y20));
        public IIoPoint Y21 => CreateIOPoint(2, 1, IoModes.output, nameof(Y21));
        public IIoPoint Y22 => CreateIOPoint(2, 2, IoModes.output, nameof(Y22));
        public IIoPoint Y23 => CreateIOPoint(2, 3, IoModes.output, nameof(Y23));
        public IIoPoint Y24 => CreateIOPoint(2, 4, IoModes.output, nameof(Y24));
        public IIoPoint Y25 => CreateIOPoint(2, 5, IoModes.output, nameof(Y25));
        public IIoPoint Y26 => CreateIOPoint(2, 6, IoModes.output, nameof(Y26));
        public IIoPoint Y27 => CreateIOPoint(2, 7, IoModes.output, nameof(Y27));

        public IIoPoint Y30 => CreateIOPoint(3, 0, IoModes.output, nameof(Y30));
        public IIoPoint Y31 => CreateIOPoint(3, 1, IoModes.output, nameof(Y31));
        public IIoPoint Y32 => CreateIOPoint(3, 2, IoModes.output, nameof(Y32));
        public IIoPoint Y33 => CreateIOPoint(3, 3, IoModes.output, nameof(Y33));
        public IIoPoint Y34 => CreateIOPoint(3, 4, IoModes.output, nameof(Y34));
        public IIoPoint Y35 => CreateIOPoint(3, 5, IoModes.output, nameof(Y35));
        public IIoPoint Y36 => CreateIOPoint(3, 6, IoModes.output, nameof(Y36));
        public IIoPoint Y37 => CreateIOPoint(3, 7, IoModes.output, nameof(Y37));

        public IIoPoint Y40 => CreateIOPoint(4, 0, IoModes.output, nameof(Y40));
        public IIoPoint Y41 => CreateIOPoint(4, 1, IoModes.output, nameof(Y41));
        public IIoPoint Y42 => CreateIOPoint(4, 2, IoModes.output, nameof(Y42));
        public IIoPoint Y43 => CreateIOPoint(4, 3, IoModes.output, nameof(Y43));
        public IIoPoint Y44 => CreateIOPoint(4, 4, IoModes.output, nameof(Y44));
        public IIoPoint Y45 => CreateIOPoint(4, 5, IoModes.output, nameof(Y45));
        public IIoPoint Y46 => CreateIOPoint(4, 6, IoModes.output, nameof(Y46));
        public IIoPoint Y47 => CreateIOPoint(4, 7, IoModes.output, nameof(Y47));

        public IIoPoint Y50 => CreateIOPoint(5, 0, IoModes.output, nameof(Y50));
        public IIoPoint Y51 => CreateIOPoint(5, 1, IoModes.output, nameof(Y51));
        public IIoPoint Y52 => CreateIOPoint(5, 2, IoModes.output, nameof(Y52));
        public IIoPoint Y53 => CreateIOPoint(5, 3, IoModes.output, nameof(Y53));
        public IIoPoint Y54 => CreateIOPoint(5, 4, IoModes.output, nameof(Y54));
        public IIoPoint Y55 => CreateIOPoint(5, 5, IoModes.output, nameof(Y55));
        public IIoPoint Y56 => CreateIOPoint(5, 6, IoModes.output, nameof(Y56));
        public IIoPoint Y57 => CreateIOPoint(5, 7, IoModes.output, nameof(Y57));

        public IIoPoint Y60 => CreateIOPoint(6, 0, IoModes.output, nameof(Y60));
        public IIoPoint Y61 => CreateIOPoint(6, 1, IoModes.output, nameof(Y61));
        public IIoPoint Y62 => CreateIOPoint(6, 2, IoModes.output, nameof(Y62));
        public IIoPoint Y63 => CreateIOPoint(6, 3, IoModes.output, nameof(Y63));
        public IIoPoint Y64 => CreateIOPoint(6, 4, IoModes.output, nameof(Y64));
        public IIoPoint Y65 => CreateIOPoint(6, 5, IoModes.output, nameof(Y65));
        public IIoPoint Y66 => CreateIOPoint(6, 6, IoModes.output, nameof(Y66));
        public IIoPoint Y67 => CreateIOPoint(6, 7, IoModes.output, nameof(Y67));
    }
}
