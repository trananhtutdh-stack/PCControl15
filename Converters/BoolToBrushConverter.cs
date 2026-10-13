using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCControl.Converters
{
    public class BoolToBrushConverter : IValueConverter
    {
        public Brush TrueBrush { get; set; } = Brushes.LimeGreen;

        public Brush FalseBrush { get; set; } = Brushes.Gray;

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not bool state)
                return FalseBrush;

            return state
                ? TrueBrush
                : FalseBrush;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return Binding.DoNothing;
        }
    }
}