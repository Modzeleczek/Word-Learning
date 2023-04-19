using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Word_Learning.MVVM.View.Converters
{
    public class LevelToBrushConverter : IValueConverter
    {
        private static readonly LevelToBrushConverter instance = new LevelToBrushConverter();
        public static LevelToBrushConverter Instance { get { return instance; } }

        private LevelToBrushConverter() { }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) // throw new ArgumentException("Value is null.");
                return DependencyProperty.UnsetValue;
            var fileNameMap = new string[] { "crying", "emoji", "expressionless", "happy", "grinning" };
            // For use with UriKind.Relative, the path cannot start with '/'.
            var filePath = $"Images/{fileNameMap[(int)value]}.png";
            return new ImageBrush(new BitmapImage(new Uri(filePath, UriKind.Relative)));
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
