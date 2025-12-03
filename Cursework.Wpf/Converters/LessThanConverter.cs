using System;
using System.Globalization;
using System.Windows.Data;

namespace Cursework.Wpf.Converters
{
    public class LessThanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
            {
                return false;
            }

            if (double.TryParse(value.ToString(), NumberStyles.Any, culture, out var actual)
                && double.TryParse(parameter.ToString(), NumberStyles.Any, culture, out var limit))
            {
                return actual < limit;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}
