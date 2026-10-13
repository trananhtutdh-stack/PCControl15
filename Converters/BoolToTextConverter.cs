using System;
using System.Globalization;
using System.Windows.Data;

namespace PCControl.Converters
{
    public class BoolToTextConverter : IValueConverter
    {
        public string TrueText { get; set; } = "ON";

        public string FalseText { get; set; } = "OFF";

        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value is bool b && b
                ? TrueText
                : FalseText;
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