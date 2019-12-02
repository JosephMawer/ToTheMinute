using System;
using Windows.UI.Xaml.Data;

namespace HoursTracker.Converters
{
    public class StringFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var format = parameter as string;
            if (!String.IsNullOrEmpty(format))
            {
                var formatted = String.Format("{0:" + format + "}", value);
                return formatted;
            }
                
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
