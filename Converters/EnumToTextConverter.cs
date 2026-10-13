using System;
using System.Globalization;
using System.Windows.Data;

namespace PCControl.Converters
{
    public class EnumToTextConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            return value?.ToString() ?? string.Empty;
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