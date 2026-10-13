using System;
using System.Globalization;
using System.Windows.Data;

namespace PCControl.Converters
{
    public class JogLevelToBoolConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;

            if (!int.TryParse(value.ToString(), out int level))
                return false;

            if (!int.TryParse(parameter.ToString(), out int targetLevel))
                return false;

            return level == targetLevel;
        }

        public object ConvertBack(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            // JogLevel được thay đổi bởi Command trong ViewModel.
            return Binding.DoNothing;
        }
    }
}