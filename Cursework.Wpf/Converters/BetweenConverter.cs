using System;
using System.Globalization;
using System.Windows.Data;

namespace Cursework.Wpf.Converters
{
    public class BetweenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            var parts = parameter.ToString()?.Split(',');
            if (parts is { Length: 2 }
                && double.TryParse(value.ToString(), NumberStyles.Any, culture, out var actual)
                && double.TryParse(parts[0], NumberStyles.Any, culture, out var min)
                && double.TryParse(parts[1], NumberStyles.Any, culture, out var max))
            {
                return actual >= min && actual <= max;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
