using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace NY.Dataverse.LINQPadDriver
{
    public class BooleanToVisibilityInverseConvertor : IValueConverter
    {
        public object Convert(object value, Type targetType, object param, CultureInfo culture) => (bool)value ? Visibility.Collapsed : Visibility.Visible;

        public object ConvertBack(object value, Type targetType, object param, CultureInfo culture) => !(value?.Equals(param)).GetValueOrDefault() ? param : Binding.DoNothing;
    }
}
