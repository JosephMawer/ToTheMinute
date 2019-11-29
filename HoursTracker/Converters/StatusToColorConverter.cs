﻿using System;
using Windows.UI;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace HoursTracker.Converters
{
    // Custom class implements the IValueConverter interface.
    class StatusToColorConverter : IValueConverter
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
                    return new SolidColorBrush(Colors.LimeGreen);
               
                case false:
                    return new SolidColorBrush(Colors.Red);
         
                default:
                    throw new Exception("Status To Color Converter failed");
            }
        }

        // ConvertBack is not implemented for a OneWay binding.
        public object ConvertBack(object value, Type targetType,
            object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
