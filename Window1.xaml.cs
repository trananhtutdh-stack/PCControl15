using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace PCControl
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public string file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Motion.ini");

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern long WritePrivateProfileString(string section, string key, string value, string file);

        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder value, int size, string file);


        public Window1()
        {
            InitializeComponent();
        }

        #region Motion

        protected class Motion
        {
            public double Speed;
            public double Accel;
            public double Decel;
            public double HomeSpeed;
        }

        Motion GetUI()
        {
            return new Motion()
            {
                Speed = double.Parse(txtSpeed.Text),
                Accel = double.Parse(txtAccel.Text),
                Decel = double.Parse(txtDecel.Text),
                HomeSpeed = double.Parse(txtHome.Text)
            };
        }

        void SetUI(Motion p)
        {
            txtSpeed.Text = p.Speed.ToString();
            txtAccel.Text = p.Accel.ToString();
            txtDecel.Text = p.Decel.ToString();
            txtHome.Text = p.HomeSpeed.ToString();
        }

        #endregion

        void SaveByAPI(Motion p)
        {
            WritePrivateProfileString("Axis0", "Speed", p.Speed.ToString(), file);
            WritePrivateProfileString("Axis0", "Accel", p.Accel.ToString(), file);
            WritePrivateProfileString("Axis0", "Decel", p.Decel.ToString(), file);
            WritePrivateProfileString("Axis0", "HomeSpeed", p.HomeSpeed.ToString(), file);
        }

        Motion LoadByAPI()
        {
            Motion p = new Motion();
            p.Speed = ReadDoubleAPI("Axis0", "Speed");
            p.Accel = ReadDoubleAPI("Axis0", "Accel");
            p.Decel = ReadDoubleAPI("Axis0", "Decel");
            p.HomeSpeed = ReadDoubleAPI("Axis0", "HomeSpeed");
            return p;
        }

        double ReadDoubleAPI(string section, string key)
        {
            StringBuilder sb = new StringBuilder(100);

            GetPrivateProfileString(section, key, "0", sb, 100, file);

            return double.Parse(sb.ToString());
        }


        void SaveByCustom(Motion p)
        {
            List<string> lines = new List<string>();

            lines.Add("[Axis0]");
            lines.Add("Speed=" + p.Speed);
            lines.Add("Accel=" + p.Accel);
            lines.Add("Decel=" + p.Decel);
            lines.Add("HomeSpeed=" + p.HomeSpeed);

            File.WriteAllLines(file, lines);
        }

        Motion LoadByCustom()
        {
            Motion p = new Motion();

            if (!File.Exists(file))
                return p;

            var lines = File.ReadAllLines(file);

            Dictionary<string, string> dic = new Dictionary<string, string>();

            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                    continue;

                if (!line.Contains("="))
                    continue;

                var sp = line.Split('=');

                dic[sp[0]] = sp[1];
            }

            p.Speed = double.Parse(dic["Speed"]);
            p.Accel = double.Parse(dic["Accel"]);
            p.Decel = double.Parse(dic["Decel"]);
            p.HomeSpeed = double.Parse(dic["HomeSpeed"]);

            return p;
        }

        private void SaveApi_Click(object sender, RoutedEventArgs e)
        {
            SaveByAPI(GetUI());
            txtLog.Text = "Lưu bằng WinAPI";
        }

        private void LoadApi_Click(object sender, RoutedEventArgs e)
        {
            SetUI(LoadByAPI());
            txtLog.Text = "Load bằng WinAPI";
        }

        private void SaveCustom_Click(object sender, RoutedEventArgs e)
        {
            SaveByCustom(GetUI());
            txtLog.Text = "Lưu bằng hàm tự viết";
        }

        private void LoadCustom_Click(object sender, RoutedEventArgs e)
        {
            SetUI(LoadByCustom());
            txtLog.Text = "Load bằng hàm tự viết";
        }
    }
}
