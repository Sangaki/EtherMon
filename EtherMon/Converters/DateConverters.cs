using System;
using System.Diagnostics;
using System.Globalization;
using Xamarin.Forms;

namespace EtherMon.Converters
{
    public class UnixToDateConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(value is double ? (double) value : 0).ToLocalTime();
            return dtDateTime.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Conversion not allowed");
        }
    }
    
    
    public class UnixToMinutesFromNowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Unix timestamp is seconds past epoch
            var dtDateTime = new DateTime(1970,1,1,0,0,0,0,System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(value is double ? (double) value : 0).ToLocalTime();
            var ts = DateTime.Now - dtDateTime;
            return Math.Round(ts.TotalMinutes);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Conversion not allowed");
        }
    }
}