using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Word_Learning.MVVM.View
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
            var colorMap = new Color[] { // poziomy nauczenia słowa
                Color.FromRgb(255/1, 255/4, 0), // czerwony, jeżeli użytkownik 0 razy z rzędu dobrze dopasował słowo do jego definicji
                Color.FromRgb(255/2, 255/3, 0), // 1 raz
                Color.FromRgb(255/3, 255/2, 0), // 2 razy
                Color.FromRgb(255/4, 255/1, 0) // 3 razy
            };
            return new SolidColorBrush(colorMap[(int)value]);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
