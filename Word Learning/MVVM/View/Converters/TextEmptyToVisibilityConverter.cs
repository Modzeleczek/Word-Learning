using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Word_Learning.MVVM.View.Converters
{
    public class TextEmptyToVisibilityConverter : IValueConverter
    {
        private static readonly TextEmptyToVisibilityConverter instance = new TextEmptyToVisibilityConverter();
        public static TextEmptyToVisibilityConverter Instance { get { return instance; } }

        private TextEmptyToVisibilityConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            var strValue = (string)value;
            if (strValue.Length == 0) return Visibility.Collapsed;
            else return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
