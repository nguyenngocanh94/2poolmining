using System;
using System.Windows.Data;
using System.Windows.Media;

namespace Chia2Pool.Common
{
    public class ColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string text = (string)value;
            switch (text)
            {
                case "WARN":
                    return Brushes.Yellow;
                case "INFO":
                    return Brushes.Green;
                default:
                    return Brushes.Red;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return null;
        }
    }
}