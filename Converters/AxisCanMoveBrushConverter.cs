using AutoVision.Service.IPC;
using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace PCControl.Converters
{
    public class AxisCanMoveBrushConverter : IValueConverter
    {
        public object Convert(
            object value,
            Type targetType,
            object parameter,
            CultureInfo culture)
        {
            if (value is not IAxis axis)
                return Brushes.Gray;

            bool canMove =
                axis.IsServoON &&
                !axis.IsBusy &&
                !axis.ExternalPositiveLS &&
                !axis.ExternalNegativeLS &&
                axis.ErrorCode == 0;

            return canMove
                ? Brushes.LimeGreen
                : Brushes.Red;
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