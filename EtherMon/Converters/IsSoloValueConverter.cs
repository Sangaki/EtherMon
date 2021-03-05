using System;
using System.Globalization;
using EtherMon.Models;
using Xamarin.Forms;

namespace EtherMon.Converters
{
    public class IsSoloValueConverter: IValueConverter
    {
        public BoolIsSoloScheme IsSoloScheme = new BoolIsSoloScheme("Solo", "Pool");
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return IsSoloScheme.GetIsSolo(value as bool?);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new Exception("Conversion not allowed");
        }
    }
}