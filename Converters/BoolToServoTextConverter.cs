using System;
using System.Globalization;
using System.Windows.Data;

namespace PCControl.Converters
{
    public sealed class BoolToServoTextConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value is true
                ? "SERVO ON"
                : "SERVO OFF";
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}