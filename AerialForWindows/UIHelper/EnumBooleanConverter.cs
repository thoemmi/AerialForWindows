using System;
using System.Windows;
using System.Windows.Data;

namespace AerialForWindows.UIHelper {
    /// <summary>
    /// Converter to help binding an enum to a RadioButton. See http://stackoverflow.com/a/406798/4747.
    /// </summary>
    public class EnumBooleanConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            if (!Enum.IsDefined(value.GetType(), value))
                return DependencyProperty.UnsetValue;

            var parameterString = parameter as string;
            if (parameterString == null)
                return DependencyProperty.UnsetValue;

            var parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
            var parameterString = parameter as string;
            return parameterString == null ? DependencyProperty.UnsetValue : Enum.Parse(targetType, parameterString);
        }
    }
}