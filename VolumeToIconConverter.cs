using System;
using System.Globalization;
using System.Windows.Data;

namespace VolumeController
{
    public class VolumeToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double volume)
            {
                if (volume <= 0) return "Zero";
                if (volume <= 33) return "Low";
                if (volume <= 66) return "Medium";
                return "High";
            }
            return "High";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
