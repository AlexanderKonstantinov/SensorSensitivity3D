using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace SensorSensitivity3D.Helpers
{
    [ValueConversion(typeof(bool), typeof(Thickness))]
    public class MarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => new Thickness(0, 0, (bool) value ? 350 : 0, 0);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    [ValueConversion(typeof(object), typeof(Visibility))]
    public class NullToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public NullToVisibilityConverter()
        {
            TrueValue = Visibility.Collapsed;
            FalseValue = Visibility.Visible;
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

    [ValueConversion(typeof(object), typeof(bool))]
    public class NullToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => value != null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => value;
    }

    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : IValueConverter
    {
        public Visibility TrueValue { get; set; }
        public Visibility FalseValue { get; set; }

        public BoolToVisibilityConverter()
        {
            TrueValue = Visibility.Visible;
            FalseValue = Visibility.Collapsed;
        }

        /// <summary>
        /// Преобразует логическую переменную в свойство Visibility
        /// </summary>
        /// <returns>
        /// По умолчанию
        /// Visibility.Visible, если объект true
        /// Visibility.Collapsed, если объект false
        /// </returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => (value is bool && (bool)value) ? TrueValue : FalseValue;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => (Visibility) value == TrueValue;
    }


    [ValueConversion(typeof(bool), typeof(string))]
    public class BoolToStringConverter : IValueConverter
    {
        public string TrueValue { get; set; }
        public string FalseValue { get; set; }

        /// <summary>
        /// Преобразует логическую переменную в строку
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !(value is bool) 
                ? string.Empty
                : ((bool)value ? TrueValue : FalseValue);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }

    [ValueConversion(typeof(string), typeof(Color))]
    public class StringToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = System.Drawing.ColorTranslator.FromHtml(value.ToString());

            return new SolidColorBrush(Color.FromArgb(color.A, color.R, color.G, color.B));


            //var color = value is null
            //    ? Color.FromRgb(0, 0, 0)
            //    : (SolidColorBrush)ColorConverter.ConvertFromString(value.ToString());

            //return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value.ToString();
        }
    }

    [ValueConversion(typeof(System.Drawing.Color), typeof(Color))]
    public class DrawingColorToMediaColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color = (System.Drawing.Color)value;
            return System.Drawing.ColorTranslator.ToHtml(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Drawing.ColorTranslator.FromHtml(value.ToString());
        }
    }

    [ValueConversion(typeof(string), typeof(Visibility))]
    public class StringToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => string.IsNullOrEmpty((string)value) ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}