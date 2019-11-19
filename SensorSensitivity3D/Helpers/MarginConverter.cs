using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SensorSensitivity3D.Helpers
{
    [ValueConversion(typeof(Control), typeof(Thickness))]
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => new Thickness(0, 0, System.Convert.ToBoolean(value) ? 325 : 0, 0);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
