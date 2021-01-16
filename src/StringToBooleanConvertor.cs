using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace NY.Dataverse.LINQPadDriver
{
    public class StringToBooleanConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture) => value?.Equals(param) ?? false;

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture) => (bool)value ? param : Binding.DoNothing;
    }
}
