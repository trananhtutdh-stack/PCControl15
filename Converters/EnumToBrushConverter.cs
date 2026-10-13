using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCControl.Converters
{
    public class EnumToBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null)
                return Brushes.Gray;

            return value.ToString() switch
            {
                "Ready" => Brushes.LimeGreen,
                "Busy" => Brushes.Gold,
                "Error" => Brushes.Red,
                "Stop" => Brushes.Gray,

                _ => Brushes.Gray
            };
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