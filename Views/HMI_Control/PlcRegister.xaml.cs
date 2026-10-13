using System;
using System.Linq;
using System.Reactive.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Automation.Text;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using PLCVir.Device;

namespace PCControl.Views
{
    public enum RegisterDataType
    {
        Int,
        Double
    }
    public enum ValueMode
    {
        Device,
        Binding
    }

    public partial class PlcRegister : UserControl
    {
        private bool _isResizingLayout = false;

        private string _lastText = "";
        private FieldInfo _field;
        private string _prefix;
        private bool _pendingUpdate;
        private bool _isEditing;

        private static readonly Brush BrushZero = Brushes.LightGray;
        private static readonly Brush BrushAlarm = Brushes.Red;
        private static readonly Brush BrushNormal = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#CCFFFF"));
        public static IObservable<long> Tick50ms = Observable.Interval(TimeSpan.FromMilliseconds(50)).Publish().RefCount();
        private IDisposable? _sub;

        public PlcRegister()
        {
            InitializeComponent();
            Loaded += PlcRegister_Loaded;
            Unloaded += PlcRegister_Unloaded;

            if (BrushNormal.CanFreeze)
                BrushNormal.Freeze();
        }

        private void InitHeavy()
        {
            var globType = typeof(GlobDev);

            _typeD = GetNestedTypeIfExists(globType, "D");
            _typeZR = GetNestedTypeIfExists(globType, "ZR");
            _typeE = GetNestedTypeIfExists(globType, "E");
            BuildDeviceCache();

            UpdateDisplay(true);
        }
        private void Tb_TextChanged(object sender, TextChangedEventArgs e)
        {
        }
        public static readonly DependencyProperty ValueFontSizeMultiplierProperty =
            DependencyProperty.Register("ValueFontSizeMultiplier", typeof(double), typeof(PlcRegister),
                new PropertyMetadata(1.2));

        public double ValueFontSizeMultiplier
        {
            get => (double)GetValue(ValueFontSizeMultiplierProperty);
            set => SetValue(ValueFontSizeMultiplierProperty, value);
        }

        public static readonly DependencyProperty UnitFontSizeMultiplierProperty =
            DependencyProperty.Register("UnitFontSizeMultiplier", typeof(double), typeof(PlcRegister),
                new PropertyMetadata(1.0));

        public double UnitFontSizeMultiplier
        {
            get => (double)GetValue(UnitFontSizeMultiplierProperty);
            set => SetValue(UnitFontSizeMultiplierProperty, value);
        }

        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(double), typeof(PlcRegister),
                new PropertyMetadata(double.MaxValue));

        public double MaxValue
        {
            get => (double)GetValue(MaxValueProperty);
            set => SetValue(MaxValueProperty, value);
        }

        public static readonly DependencyProperty MinValueProperty =
            DependencyProperty.Register("MinValue", typeof(double), typeof(PlcRegister),
                new PropertyMetadata(double.MinValue));

        public double MinValue
        {
            get => (double)GetValue(MinValueProperty);
            set => SetValue(MinValueProperty, value);
        }

        public static readonly DependencyProperty AlarmValueUpperProperty =
            DependencyProperty.Register(nameof(AlarmValueUpper), typeof(double), typeof(PlcRegister),
                new PropertyMetadata((double)999999));

        public double AlarmValueUpper
        {
            get => (double)GetValue(AlarmValueUpperProperty);
            set => SetValue(AlarmValueUpperProperty, value);
        }

        public static readonly DependencyProperty MaxDigitsProperty =
            DependencyProperty.Register(nameof(MaxDigits), typeof(int), typeof(PlcRegister),
                new PropertyMetadata(20));

        public int MaxDigits
        {
            get => (int)GetValue(MaxDigitsProperty);
            set => SetValue(MaxDigitsProperty, value);
        }

        public static readonly DependencyProperty DeviceProperty =
            DependencyProperty.Register("Device", typeof(string), typeof(PlcRegister),
                new PropertyMetadata("", OnDeviceChanged));

        public string Device
        {
            get => (string)GetValue(DeviceProperty);
            set => SetValue(DeviceProperty, value);
        }
        public static readonly DependencyProperty ValueModeProperty =
    DependencyProperty.Register(
        nameof(ValueMode),
        typeof(ValueMode),
        typeof(PlcRegister),
        new PropertyMetadata(ValueMode.Device));

        public ValueMode ValueMode
        {
            get => (ValueMode)GetValue(ValueModeProperty);
            set => SetValue(ValueModeProperty, value);
        }
        public static readonly DependencyProperty BindingValueProperty =
    DependencyProperty.Register(
        nameof(BindingValue),
        typeof(double),
        typeof(PlcRegister),
        new FrameworkPropertyMetadata(
            0.0,
            FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnBindingValueChanged));

        public double BindingValue
        {
            get => (double)GetValue(BindingValueProperty);
            set => SetValue(BindingValueProperty, value);
        }
        public static readonly DependencyProperty EnableInputProperty =
            DependencyProperty.Register("EnableInput", typeof(bool), typeof(PlcRegister),
                new PropertyMetadata(true, OnEnableInputChanged));

        public bool EnableInput
        {
            get => (bool)GetValue(EnableInputProperty);
            set => SetValue(EnableInputProperty, value);
        }

        public static readonly DependencyProperty FractionalDigitsProperty =
            DependencyProperty.Register("FractionalDigits", typeof(int), typeof(PlcRegister),
                new PropertyMetadata(0));

        public int FractionalDigits
        {
            get => (int)GetValue(FractionalDigitsProperty);
            set => SetValue(FractionalDigitsProperty, value);
        }

        public static readonly DependencyProperty DataTypeProperty =
            DependencyProperty.Register("DataType", typeof(RegisterDataType), typeof(PlcRegister),
                new PropertyMetadata(RegisterDataType.Int));

        public RegisterDataType DataType
        {
            get => (RegisterDataType)GetValue(DataTypeProperty);
            set => SetValue(DataTypeProperty, value);
        }

        public static readonly DependencyProperty UnitProperty =
            DependencyProperty.Register("Unit", typeof(string), typeof(PlcRegister),
                new PropertyMetadata("", OnUnitChanged));

        public string Unit
        {
            get => (string)GetValue(UnitProperty);
            set => SetValue(UnitProperty, value);
        }

        public static readonly DependencyProperty MaxFontSizeProperty =
            DependencyProperty.Register("MaxFontSize", typeof(double), typeof(PlcRegister),
                new PropertyMetadata(30.0));

        public double MaxFontSize
        {
            get => (double)GetValue(MaxFontSizeProperty);
            set => SetValue(MaxFontSizeProperty, value);
        }

        public bool IsReadOnly => !EnableInput;
        private static void OnBindingValueChanged(
    DependencyObject d,
    DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcRegister pr)
            {
                if (pr.ValueMode == ValueMode.Binding)
                {
                    pr.UpdateDisplay(true);
                }
            }
        }
        public static readonly DependencyProperty TextBoxBackgroundProperty =
    DependencyProperty.Register(
        nameof(TextBoxBackground),
        typeof(Brush),
        typeof(PlcRegister),
        new PropertyMetadata(Brushes.White));

        public Brush TextBoxBackground
        {
            get => (Brush)GetValue(TextBoxBackgroundProperty);
            set => SetValue(TextBoxBackgroundProperty, value);
        }
        public static readonly DependencyProperty TextBoxForegroundProperty =
    DependencyProperty.Register(
        nameof(TextBoxForeground),
        typeof(Brush),
        typeof(PlcRegister),
        new PropertyMetadata(Brushes.Black));

        public Brush TextBoxForeground
        {
            get => (Brush)GetValue(TextBoxForegroundProperty);
            set => SetValue(TextBoxForegroundProperty, value);
        }
        private static void OnDeviceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var pr = d as PlcRegister;
            if (pr == null) return;

            pr.BuildDeviceCache();
            pr._lastText = "";
            //pr.UpdateDisplay(true);
        }

        private static void OnEnableInputChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcRegister pr)
                pr.Tb.IsReadOnly = !(bool)e.NewValue;
        }

        private static void OnUnitChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is PlcRegister pr)
                pr.TxtUnit.Text = e.NewValue?.ToString() ?? "";
        }

        private Type _typeD, _typeZR, _typeE;
        private EventInfo _evD, _evZR, _evE;

        private void PlcRegister_Loaded(object sender, RoutedEventArgs e)
        {
            Tb.IsReadOnly = !EnableInput;

            if (ValueMode == ValueMode.Binding)
            {
                UpdateDisplay(true);
                return;
            }

            _sub ??= Tick50ms.Subscribe(_ => App.UIContext?.Post(__ => UpdateDisplay(), null));
            Dispatcher.BeginInvoke(new Action(() =>
            {
                InitHeavy();
            }), DispatcherPriority.ContextIdle);
        }

        private void PlcRegister_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachStaticChanged(_evD);
            DetachStaticChanged(_evZR);
            DetachStaticChanged(_evE);
            _sub?.Dispose();
            _sub = null;
        }

        private void BuildDeviceCache()
        {
            _prefix = new string((Device ?? "").TakeWhile(c => !char.IsDigit(c)).ToArray());
            if (string.IsNullOrEmpty(_prefix)) _prefix = "D";
            _prefix = _prefix.ToUpper();

            Type t = _prefix switch
            {
                "D" => _typeD,
                "ZR" => _typeZR,
                "E" => _typeE,
                _ => GetNestedTypeIfExists(typeof(GlobDev), _prefix)
            };

            _field = t?.GetField("Vals", BindingFlags.Public | BindingFlags.Static);
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            if (!_pendingUpdate) return;
            if (_isEditing) return;

            _pendingUpdate = false;
            //UpdateDisplay();
        }

        private void Tb_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _isResizingLayout = true;
            AutoScaleFont();
            _isResizingLayout = false;
        }

        private void AutoScaleFont()
        {
            if (Tb == null) return;

            double maxSize = MaxFontSize;
            double minSize = 10;

            double availableWidth = Tb.ActualWidth - 10;
            double availableHeight = Tb.ActualHeight - 6;

            if (availableWidth <= 0 || availableHeight <= 0) return;

            string text = string.IsNullOrEmpty(Tb.Text) ? "0" : Tb.Text;
            double size = Math.Min(maxSize, Tb.FontSize > 0 ? Tb.FontSize : maxSize);

            while (size >= minSize)
            {
                var ft = new FormattedText(
                    text,
                    System.Globalization.CultureInfo.CurrentCulture,
                    FlowDirection.LeftToRight,
                    new Typeface(Tb.FontFamily, Tb.FontStyle, Tb.FontWeight, Tb.FontStretch),
                    size,
                    Brushes.Black,
                    VisualTreeHelper.GetDpi(this).PixelsPerDip);

                if (ft.Width <= availableWidth && ft.Height <= availableHeight)
                    break;

                size -= 1.0;
            }

            if (_isResizingLayout)
            {
                if (size < Tb.FontSize || Tb.FontSize <= 0)
                    Tb.FontSize = size;
            }
            else
            {
                Tb.FontSize = size;
            }

            TxtUnit.FontSize = Tb.FontSize * UnitFontSizeMultiplier;
        }

        private void Tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !IsValidInput(e.Text);
        }

        private void Tb_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!e.DataObject.GetDataPresent(DataFormats.Text))
            {
                e.CancelCommand();
                return;
            }

            string pasted = (string)e.DataObject.GetData(DataFormats.Text);
            if (!IsValidInput(pasted))
                e.CancelCommand();
        }

        private bool IsValidInput(string input)
        {
            foreach (char c in input)
            {
                if (char.IsDigit(c)) continue;
                if (c == '.' || c == ',') continue;

                if (c == '-')
                {
                    int caret = Tb.SelectionStart;
                    bool insertingAtFirst = (caret == 0);
                    bool alreadyHasMinus = Tb.Text.StartsWith("-");

                    if (insertingAtFirst && !alreadyHasMinus)
                        continue;

                    return false;
                }
                return false;
            }

            string next = Tb.Text.Insert(Tb.SelectionStart, input);
            int digitCount = next.Count(char.IsDigit);

            if (digitCount > MaxDigits)
                return false;

            return true;
        }

        private void Tb_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                CommitText();
                MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                e.Handled = true;
            }
        }

        private void Tb_LostFocus(object sender, RoutedEventArgs e)
        {
            _isEditing = false;
            CommitText();
        }

        private void Tb_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            _isEditing = true;
        }

        private void CommitText()
        {
            if (!EnableInput)
            {
                _lastText = "";
                //UpdateDisplay(force: true);
                return;
            }

            if (ValueMode == ValueMode.Device &&
    string.IsNullOrEmpty(Device))
                return;

            string s = Tb.Text.Trim();
            string dev = Device.ToUpper();

            try
            {
                if (double.TryParse(s, out double v))
                {
                    if (v > MaxValue) v = MaxValue;
                    if (v < MinValue) v = MinValue;

                    if (ValueMode == ValueMode.Binding)
                    {
                        BindingValue = v;
                    }
                    else
                    {
                        if (dev.StartsWith("E"))
                        {
                            WriteEValue(dev, v);
                        }
                        else
                        {
                            int raw = (int)Math.Round(v * Math.Pow(10, FractionalDigits));
                            WriteIntValue(dev, raw);
                        }
                    }
                }
            }
            catch { }

            _lastText = "";
            //UpdateDisplay(force: true);
        }

        private void UpdateDisplay(bool force = false)
        {
            try
            {
                string text;

                if (ValueMode == ValueMode.Device)
                {
                    text = ReadDeviceFormatted();
                }
                else
                {
                    text = BindingValue.ToString($"F{FractionalDigits}");
                }

                if (!force && text == _lastText) return;
                _lastText = text;

                if (Tb.Text != text)
                    Tb.Text = text;

                if (double.TryParse(text, out double value))
                {
                    Brush nextBrush = value >= AlarmValueUpper
                        ? BrushAlarm
                        : value == 0
                            ? BrushZero
                            : BrushNormal;

                    if (!ReferenceEquals(Root.Background, nextBrush))
                        Root.Background = nextBrush;
                }
            }
            catch { }
        }

        private string ReadDeviceFormatted()
        {
            string dev = Device?.ToUpper() ?? "";

            if (dev.StartsWith("E"))
                return ReadEValue(dev).ToString($"F{FractionalDigits}");

            int raw = ReadIntValue(dev);
            double scaled = raw / Math.Pow(10, FractionalDigits);

            return scaled.ToString($"F{FractionalDigits}");
        }

        private double ReadEValue(string dev)
        {
            if (_field == null) return 0;

            var arr = _field.GetValue(null) as Array;
            if (arr != null &&
                int.TryParse(dev.Substring(_prefix.Length), out int idx) &&
                idx >= 0 && idx < arr.Length)
            {
                return Convert.ToDouble(arr.GetValue(idx));
            }
            return 0;
        }

        private int ReadIntValue(string dev)
        {
            if (_field == null) return 0;

            var arr = _field.GetValue(null) as Array;
            if (arr != null &&
                int.TryParse(dev.Substring(_prefix.Length), out int idx) &&
                idx >= 0 && idx < arr.Length)
            {
                return Convert.ToInt32(arr.GetValue(idx));
            }

            return 0;
        }

        private void WriteEValue(string dev, double value)
        {
            if (_field == null) return;

            var arr = _field.GetValue(null) as Array;
            if (arr != null &&
                int.TryParse(dev.Substring(_prefix.Length), out int idx) &&
                idx >= 0 && idx < arr.Length)
            {
                arr.SetValue(value, idx);
                RaiseStaticChangedIfExists(_field.DeclaringType, dev);
            }
        }

        private void WriteIntValue(string dev, int raw)
        {
            if (_field == null) return;

            var arr = _field.GetValue(null) as Array;
            if (arr != null &&
                int.TryParse(dev.Substring(_prefix.Length), out int idx) &&
                idx >= 0 && idx < arr.Length)
            {
                arr.SetValue(raw, idx);
                RaiseStaticChangedIfExists(_field.DeclaringType, dev);
            }
        }

        private void RaiseStaticChangedIfExists(Type t, string dev)
        {
            var ev = t?.GetEvent("StaticChanged", BindingFlags.Public | BindingFlags.Static);
            if (ev == null) return;

            var fi = t.GetField("StaticChanged",
                     BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                  ?? t.GetField("StaticChanged",
                     BindingFlags.Public | BindingFlags.Static);

            var dlg = fi?.GetValue(null) as MulticastDelegate;
            dlg?.DynamicInvoke(dev);
        }

        private void AttachStaticChanged(EventInfo ev)
        {
            if (ev == null) return;

            var dlg = Delegate.CreateDelegate(ev.EventHandlerType, this, nameof(HandleStaticChanged));
            ev.AddEventHandler(null, dlg);
        }

        private void DetachStaticChanged(EventInfo ev)
        {
            if (ev == null) return;

            var dlg = Delegate.CreateDelegate(ev.EventHandlerType, this, nameof(HandleStaticChanged));
            ev.RemoveEventHandler(null, dlg);
        }

        public void HandleStaticChanged(string name)
        {
            try
            {
                if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(Device))
                    return;

                if (!name.Equals(Device, StringComparison.OrdinalIgnoreCase))
                    return;

                _pendingUpdate = true;
            }
            catch { }
        }

        private Type GetNestedTypeIfExists(Type parent, string name)
        {
            try
            {
                return parent.GetNestedType(name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            }
            catch
            {
                return null;
            }
        }

        private EventInfo GetStaticEvent(Type t, string eventName)
        {
            return t?.GetEvent(eventName, BindingFlags.Public | BindingFlags.Static);
        }

        private void Tb_OpenKeyboard(object sender, RoutedEventArgs e)
        {
            if (!EnableInput)
                return;

            e.Handled = true;
            _isEditing = true;

            string dev = Device?.ToUpper() ?? "";

            if (dev.StartsWith("D") || dev.StartsWith("ZR") || dev.StartsWith("E"))
            {
                var np = new NumberPadWindow(Tb.Text, Tb);
                if (np.ShowDialog() == true)
                {
                    Tb.Text = np.ResultText;
                    CommitText();
                }

                _isEditing = false;
                return;
            }

            var kb = new KeyboardWindow(Tb.Text, Tb);
            if (kb.ShowDialog() == true)
            {
                Tb.Text = kb.ResultText;
                CommitText();
            }

            _isEditing = false;
        }
    }
}