using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace SensorSensitivity3D.Helpers
{
    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public NullToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        /// <summary>
        /// Преобразует объект в свойство Visibility
        /// </summary>
        /// <returns>
        /// /// По умолчанию
        /// Visibility.Visible, если объект не null
        /// Visibility.Collapsed, если объект null
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
            => value is null
                ? FalseValue
                : TrueValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class InvNullToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public InvNullToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        /// <summary>
        /// Преобразует объект в свойство Visibility
        /// </summary>
        /// <returns>
        /// По умолчанию
        /// Visibility.Visible, если объект null
        /// Visibility.Collapsed, если объект не null
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value is null
                ? TrueValue
                : FalseValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}