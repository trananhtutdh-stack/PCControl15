using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using AutoVision.Service.IPC;
using PCControl.Service.IPC;

namespace PCControl.Views
{
    public enum IconPosition
    {
        Left,
        Right,
        Top,
        Bottom
    }
    public enum PlcButtonMode
    {
        Momentary,
        Set,
        Reset,
        Invert
    }

    public enum ButtonShape
    {
        Rectangle,
        Circle
    }

    public enum PlcValueMode
    {
        Device,
        Binding
    }

    public partial class PlcButton : UserControl
    {
        private ImageSource ButtonIcon { get; set; }

        private readonly DispatcherTimer _refreshTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromMilliseconds(50)
        };

        private Brush ButtonBackground { get; set; } = Brushes.Gray;
        private string ButtonText { get; set; } = "";

        public PlcButton()
        {
            InitializeComponent();

            Loaded += PlcButton_Loaded;
            Unloaded += PlcButton_Unloaded;
            SizeChanged += PlcButton_SizeChanged;

            _refreshTimer.Tick += (_, _) => UpdateUI();
        }

        private void PlcButton_Loaded(object sender, RoutedEventArgs e)
        {
            _refreshTimer.Start();
            UpdateUI();
        }

        private void PlcButton_Unloaded(object sender, RoutedEventArgs e)
        {
            _refreshTimer.Stop();
        }

        public static readonly DependencyProperty IpcProperty =
            DependencyProperty.Register(
                nameof(Ipc),
                typeof(IIpc),
                typeof(PlcButton),
                new PropertyMetadata(null, OnAnyVisualPropertyChanged));

        public IIpc Ipc
        {
            get => (IIpc)GetValue(IpcProperty);
            set => SetValue(IpcProperty, value);
        }

        public static readonly DependencyProperty ValueModeProperty =
            DependencyProperty.Register(
                nameof(ValueMode),
                typeof(PlcValueMode),
                typeof(PlcButton),
                new PropertyMetadata(PlcValueMode.Device, OnAnyVisualPropertyChanged));

        public PlcValueMode ValueMode
        {
            get => (PlcValueMode)GetValue(ValueModeProperty);
            set => SetValue(ValueModeProperty, value);
        }

        public static readonly DependencyProperty BindingValueProperty =
            DependencyProperty.Register(
                nameof(BindingValue),
                typeof(bool),
                typeof(PlcButton),
                new FrameworkPropertyMetadata(
                    false,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnAnyVisualPropertyChanged));

        public bool BindingValue
        {
            get => (bool)GetValue(BindingValueProperty);
            set => SetValue(BindingValueProperty, value);
        }

        public static readonly DependencyProperty ReadBindingValueProperty =
            DependencyProperty.Register(
                nameof(ReadBindingValue),
                typeof(bool?),
                typeof(PlcButton),
                new PropertyMetadata(null, OnAnyVisualPropertyChanged));

        public bool? ReadBindingValue
        {
            get => (bool?)GetValue(ReadBindingValueProperty);
            set => SetValue(ReadBindingValueProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                nameof(Command),
                typeof(ICommand),
                typeof(PlcButton),
                new PropertyMetadata(null));

        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);
            set => SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                nameof(CommandParameter),
                typeof(object),
                typeof(PlcButton),
                new PropertyMetadata(null));

        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);
            set => SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty ShapeProperty =
            DependencyProperty.Register(
                nameof(Shape),
                typeof(ButtonShape),
                typeof(PlcButton),
                new PropertyMetadata(ButtonShape.Rectangle, OnShapeChanged));

        public ButtonShape Shape
        {
            get => (ButtonShape)GetValue(ShapeProperty);
            set => SetValue(ShapeProperty, value);
        }

        private static void OnShapeChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcButton control)
                control.UpdateCornerRadius();
        }

        private void PlcButton_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            UpdateCornerRadius();
        }

        private void UpdateCornerRadius()
        {
            if (Bd == null)
                return;

            if (Shape == ButtonShape.Circle)
            {
                double r = Math.Min(ActualWidth, ActualHeight) / 2;
                Bd.CornerRadius = new CornerRadius(r);
            }
            else
            {
                Bd.CornerRadius = new CornerRadius(6);
            }
        }

        public static readonly DependencyProperty IconPositionProperty =
            DependencyProperty.Register(
                nameof(IconPosition),
                typeof(IconPosition),
                typeof(PlcButton),
                new PropertyMetadata(IconPosition.Left, OnIconLayoutChanged));

        public IconPosition IconPosition
        {
            get => (IconPosition)GetValue(IconPositionProperty);
            set => SetValue(IconPositionProperty, value);
        }

        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(double),
                typeof(PlcButton),
                new PropertyMetadata(18.0, OnIconLayoutChanged));

        public double IconSize
        {
            get => (double)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }

        private static void OnIconLayoutChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcButton btn)
                btn.UpdateIconLayout();
        }

        private void UpdateIconLayout()
        {
            if (LayoutPanel == null) return;

            IconImg.Width = IconSize;
            IconImg.Height = IconSize;

            LayoutPanel.Children.Clear();

            switch (IconPosition)
            {
                case IconPosition.Left:
                    LayoutPanel.Orientation = Orientation.Horizontal;
                    LayoutPanel.Children.Add(IconImg);
                    LayoutPanel.Children.Add(Txt);
                    break;

                case IconPosition.Right:
                    LayoutPanel.Orientation = Orientation.Horizontal;
                    LayoutPanel.Children.Add(Txt);
                    LayoutPanel.Children.Add(IconImg);
                    break;

                case IconPosition.Top:
                    LayoutPanel.Orientation = Orientation.Vertical;
                    LayoutPanel.Children.Add(IconImg);
                    LayoutPanel.Children.Add(Txt);
                    break;

                case IconPosition.Bottom:
                    LayoutPanel.Orientation = Orientation.Vertical;
                    LayoutPanel.Children.Add(Txt);
                    LayoutPanel.Children.Add(IconImg);
                    break;
            }
        }

        public static readonly DependencyProperty IconOnProperty = DependencyProperty.Register(
            nameof(IconOn),
            typeof(ImageSource),
            typeof(PlcButton),
            new PropertyMetadata(null, (d, e) => ((PlcButton)d).UpdateUI()));

        public ImageSource IconOn
        {
            get => (ImageSource)GetValue(IconOnProperty);
            set => SetValue(IconOnProperty, value);
        }

        public static readonly DependencyProperty IconOffProperty = DependencyProperty.Register(
            nameof(IconOff),
            typeof(ImageSource),
            typeof(PlcButton),
            new PropertyMetadata(null, (d, e) => ((PlcButton)d).UpdateUI()));

        public ImageSource IconOff
        {
            get => (ImageSource)GetValue(IconOffProperty);
            set => SetValue(IconOffProperty, value);
        }

        public static readonly DependencyProperty ModeProperty =
            DependencyProperty.Register(
                nameof(Mode),
                typeof(PlcButtonMode),
                typeof(PlcButton),
                new PropertyMetadata(PlcButtonMode.Momentary));

        public PlcButtonMode Mode
        {
            get => (PlcButtonMode)GetValue(ModeProperty);
            set => SetValue(ModeProperty, value);
        }

        public static readonly DependencyProperty ReadBitProperty =
            DependencyProperty.Register(
                nameof(ReadBit),
                typeof(string),
                typeof(PlcButton),
                new PropertyMetadata(null, OnAnyVisualPropertyChanged));

        public string ReadBit
        {
            get => (string)GetValue(ReadBitProperty);
            set => SetValue(ReadBitProperty, value);
        }

        public static readonly DependencyProperty WriteBitProperty =
            DependencyProperty.Register(
                nameof(WriteBit),
                typeof(string),
                typeof(PlcButton),
                new PropertyMetadata(null));

        public string WriteBit
        {
            get => (string)GetValue(WriteBitProperty);
            set => SetValue(WriteBitProperty, value);
        }

        public static readonly DependencyProperty OpConfirmProperty =
            DependencyProperty.Register(
                nameof(OpConfirm),
                typeof(int),
                typeof(PlcButton),
                new PropertyMetadata(0));

        public int OpConfirm
        {
            get => (int)GetValue(OpConfirmProperty);
            set => SetValue(OpConfirmProperty, value);
        }

        public static readonly DependencyProperty TextOnProperty =
            DependencyProperty.Register(
                nameof(TextOn),
                typeof(string),
                typeof(PlcButton),
                new PropertyMetadata("ON", OnAnyVisualPropertyChanged));

        public string TextOn
        {
            get => (string)GetValue(TextOnProperty);
            set => SetValue(TextOnProperty, value);
        }

        public static readonly DependencyProperty TextOffProperty =
            DependencyProperty.Register(
                nameof(TextOff),
                typeof(string),
                typeof(PlcButton),
                new PropertyMetadata("OFF", OnAnyVisualPropertyChanged));

        public string TextOff
        {
            get => (string)GetValue(TextOffProperty);
            set => SetValue(TextOffProperty, value);
        }

        public static readonly DependencyProperty BackgroundOnProperty =
            DependencyProperty.Register(
                nameof(BackgroundOn),
                typeof(Brush),
                typeof(PlcButton),
                new PropertyMetadata(Brushes.LightGreen, OnAnyVisualPropertyChanged));

        public Brush BackgroundOn
        {
            get => (Brush)GetValue(BackgroundOnProperty);
            set => SetValue(BackgroundOnProperty, value);
        }

        public static readonly DependencyProperty BackgroundOffProperty =
            DependencyProperty.Register(
                nameof(BackgroundOff),
                typeof(Brush),
                typeof(PlcButton),
                new PropertyMetadata(Brushes.LightGray, OnAnyVisualPropertyChanged));

        public Brush BackgroundOff
        {
            get => (Brush)GetValue(BackgroundOffProperty);
            set => SetValue(BackgroundOffProperty, value);
        }

        private static void OnAnyVisualPropertyChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcButton control)
                control.UpdateUI();
        }

        private bool ReadValue()
        {
            if (ValueMode == PlcValueMode.Binding)
            {
                return ReadBindingValue ?? BindingValue;
            }

            return ReadBitValue(ReadBit);
        }

        private bool ReadWriteValue()
        {
            if (ValueMode == PlcValueMode.Binding)
            {
                return BindingValue;
            }

            return ReadBitValue(WriteBit);
        }

        private void WriteValue(bool value)
        {
            if (ValueMode == PlcValueMode.Binding)
            {
                BindingValue = value;
                return;
            }

            WriteBitValue(value);
        }

        private string NormalizeBit(string bit)
        {
            if (string.IsNullOrWhiteSpace(bit))
                return bit;

            bit = bit.Trim().ToUpper();

            if (bit.Length == 3 &&
                (bit[0] == 'X' || bit[0] == 'Y') &&
                bit[1] == '0')
            {
                return $"{bit[0]}{bit[2]}";
            }

            return bit;
        }

        private bool ReadBitValue(string bit)
        {
            bit = NormalizeBit(bit);

            if (string.IsNullOrEmpty(bit))
                return false;

            string prefix = bit.Substring(0, 1).ToUpper();
            string hex = bit.Substring(1);

            if (Ipc == null)
                return false;

            if (prefix == "X")
            {
                if (hex.Length == 2)
                {
                    int slave = ConvertHex(hex[0]);
                    int b = ConvertHex(hex[1]);

                    return Ipc.IO.ReadInput(slave, b);
                }

                if (hex.Length == 1)
                {
                    int slave = ConvertHex('0');
                    int b = ConvertHex(hex[0]);
                    return Ipc.IO.ReadInput(slave, b);
                }
            }

            if (prefix == "Y")
            {
                if (hex.Length == 2)
                {
                    int slave = ConvertHex(hex[0]);
                    int b = ConvertHex(hex[1]);

                    return Ipc.IO.ReadOutput(slave, b);
                }

                if (hex.Length == 1)
                {
                    int slave = ConvertHex('0');
                    int b = ConvertHex(hex[0]);

                    return Ipc.IO.ReadOutput(slave, b);
                }
            }

            return false;
        }

        private void WriteBitValue(bool value)
        {
            string bit = NormalizeBit(WriteBit);

            if (string.IsNullOrEmpty(bit))
                return;

            string prefix = bit.Substring(0, 1).ToUpper();
            string hex = bit.Substring(1);

            if (Ipc == null)
                return;

            if (prefix == "Y")
            {
                if (hex.Length == 2)
                {
                    int slave = ConvertHex(hex[0]);
                    int b = ConvertHex(hex[1]);

                    Ipc.IO.WriteOutput(slave, b, value);
                }

                if (hex.Length == 1)
                {
                    int slave = ConvertHex('0');
                    int b = ConvertHex(hex[0]);

                    Ipc.IO.WriteOutput(slave, b, value);
                }

                return;
            }
        }

        private int ConvertHex(char c)
        {
            return Convert.ToInt32(c.ToString(), 16);
        }

        private void UpdateUI()
        {
            if (!IsVisible)
                return;

            if (Bd == null || Txt == null)
                return;

            bool on = ReadValue();

            ButtonBackground = on ? BackgroundOn : BackgroundOff;
            ButtonText = on ? TextOn : TextOff;

            Bd.Background = ButtonBackground;
            Txt.Text = ButtonText;
            if (IconImg != null)
            {
                ImageSource icon = null;

                if (on)
                {
                    icon = IconOn ?? IconOff;
                }
                else
                {
                    icon = IconOff ?? IconOn;
                }

                if (icon != null)
                {
                    IconImg.Source = icon;
                    IconImg.Visibility = Visibility.Visible;
                }
                else
                {
                    IconImg.Source = null;
                    IconImg.Visibility = Visibility.Collapsed;
                }
            }
        }

        private void ExecutePressAction()
        {
            switch (Mode)
            {
                case PlcButtonMode.Momentary:
                case PlcButtonMode.Set:
                    WriteValue(true);
                    break;

                case PlcButtonMode.Reset:
                    WriteValue(false);
                    break;

                case PlcButtonMode.Invert:
                    WriteValue(!ReadWriteValue());
                    break;
            }

            if (Command != null)
            {
                object parameter = CommandParameter ?? this;

                if (Command.CanExecute(parameter))
                    Command.Execute(parameter);
            }

            UpdateUI();
        }

        private void ExecuteReleaseAction()
        {
            if (Mode == PlcButtonMode.Momentary)
            {
                WriteValue(false);
                UpdateUI();
            }
        }

        private void Bd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ExecutePressAction();
            Bd.CaptureMouse();
        }

        private void Bd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            ExecuteReleaseAction();
            Bd.ReleaseMouseCapture();
        }

        private void Bd_MouseMove(object sender, MouseEventArgs e)
        {
            if (Mode != PlcButtonMode.Momentary)
                return;

            if (e.LeftButton != MouseButtonState.Pressed)
                return;

            Point p = e.GetPosition(Bd);

            if (p.X < 0 ||
                p.Y < 0 ||
                p.X > Bd.ActualWidth ||
                p.Y > Bd.ActualHeight)
            {
                WriteValue(false);
                UpdateUI();
            }
        }

        private void Bd_TouchDown(object sender, TouchEventArgs e)
        {
            ExecutePressAction();
            Bd.CaptureTouch(e.TouchDevice);
        }

        private void Bd_TouchUp(object sender, TouchEventArgs e)
        {
            ExecuteReleaseAction();
            Bd.ReleaseTouchCapture(e.TouchDevice);
        }

        private void Bd_TouchMove(object sender, TouchEventArgs e)
        {
            if (Mode != PlcButtonMode.Momentary)
                return;

            Point p = e.GetTouchPoint(Bd).Position;

            if (p.X < 0 ||
                p.Y < 0 ||
                p.X > Bd.ActualWidth ||
                p.Y > Bd.ActualHeight)
            {
                WriteValue(false);
                UpdateUI();
            }
        }
    }
}
