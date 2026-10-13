using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCControl.Converters
{
    public class BoolToHomeBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            bool isHome = value is bool b && b;

            return isHome
                ? new SolidColorBrush(Color.FromRgb(40, 150, 80))
                : new SolidColorBrush(Color.FromRgb(55, 55, 55));
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