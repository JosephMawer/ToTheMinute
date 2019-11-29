using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HoursTracker.Converters
{
    public class BoolToButtonTextConverter : IValueConverter
    {
        // Define the Convert method to change a DateTime object to 
        // a month string.
        public object Convert(object value, Type targetType,
            object parameter, string language)
        {
            // The value parameter is the data from the source object.
            var v = value as bool?;
            switch (v)
            {
                case true:
                    // default return value of lime green      
                    return "Clock Out";

                case false:
                    return "Clock In";

                default:
                    throw new Exception("Status To Color Converter failed");
            }
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            return default;
        }
    }
}
