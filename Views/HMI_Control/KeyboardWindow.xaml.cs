using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace PCControl.Views
{
    public partial class KeyboardWindow : Window
    {
        [DllImport("user32.dll")]
        static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        const int WM_NCLBUTTONDOWN = 0xA1;
        static readonly IntPtr HTCAPTION = new IntPtr(0x2);

        UIElement _caller;
        bool _shift = false;

        public string ResultText { get; private set; }

        public KeyboardWindow(string initialText, UIElement caller = null)
        {
            InitializeComponent();
            _caller = caller;

            TxtDisplay.Text = initialText ?? "";
            TxtDisplay.SelectionStart = TxtDisplay.Text.Length;

            Loaded += (s, e) =>
            {
                if (_caller != null)
                    AutoPositionNearCaller(_caller);

                TxtDisplay.Focus();
            };
        }
        private void AutoPositionNearCaller(UIElement caller)
        {
            FrameworkElement fe = caller as FrameworkElement;
            if (fe == null) return;

            Point screenPoint = fe.PointToScreen(new Point(0, fe.ActualHeight));
            var source = PresentationSource.FromVisual(this);
            if (source == null)
            {
                this.Left = screenPoint.X;
                this.Top = screenPoint.Y;
                return;
            }

            var fromDevice = source.CompositionTarget.TransformFromDevice;
            Point dipPoint = fromDevice.Transform(screenPoint);

            double pad = 4;
            double targetLeft = dipPoint.X;
            double targetTop = dipPoint.Y + pad;

            double screenW = SystemParameters.PrimaryScreenWidth;
            double screenH = SystemParameters.PrimaryScreenHeight;

            if (targetTop + this.Height > screenH)
                targetTop = dipPoint.Y - this.Height - fe.ActualHeight - pad;

            if (targetLeft + this.Width > screenW)
                targetLeft = Math.Max(0, screenW - this.Width - pad);

            if (targetLeft < 0) targetLeft = 0;
            if (targetTop < 0) targetTop = 0;

            this.Left = targetLeft;
            this.Top = targetTop;
        }
        void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            SendMessage(helper.Handle, WM_NCLBUTTONDOWN, HTCAPTION, IntPtr.Zero);
        }
        void Key_Click(object sender, RoutedEventArgs e)
        {
            var b = sender as System.Windows.Controls.Button;
            if (b == null) return;

            string key = b.Content.ToString();
            key = _shift ? key.ToUpper() : key.ToLower();

            TxtDisplay.Text += key;
            TxtDisplay.SelectionStart = TxtDisplay.Text.Length;
            TxtDisplay.Focus();
        }

        void Shift_Click(object sender, RoutedEventArgs e)
        {
            _shift = !_shift;
        }

        void Backspace_Click(object sender, RoutedEventArgs e)
        {
            if (TxtDisplay.Text.Length > 0)
            {
                TxtDisplay.Text = TxtDisplay.Text.Substring(0, TxtDisplay.Text.Length - 1);
                TxtDisplay.SelectionStart = TxtDisplay.Text.Length;
                TxtDisplay.Focus();
            }
        }

        void Space_Click(object sender, RoutedEventArgs e)
        {
            TxtDisplay.Text += " ";
            TxtDisplay.SelectionStart = TxtDisplay.Text.Length;
            TxtDisplay.Focus();
        }

        void OK_Click(object sender, RoutedEventArgs e)
        {
            ResultText = TxtDisplay.Text;
            DialogResult = true;
        }

        void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
