using System;
using System.Globalization;
using System.Windows.Data;

namespace PCControl.Converters
{
    public sealed class BoolToServoHomeTextConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value is true
                ? "IN HOME"
                : "NOT IN HOME";
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
