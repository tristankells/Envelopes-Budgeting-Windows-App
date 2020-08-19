﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Envelopes.Presentation {

    /// <summary>
    /// Converts a decimal value to the either red or green. Used to set the background colour of UI elements depending on a decimal value.
    /// </summary>
    class DecimalToBackgroundColourConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var input = value as decimal? ?? 0;

            if (input < 0) {
                return "#f49e8b";
            }

            if (input > 0) {
                return "#b5e08a" ;
            }

            return DependencyProperty.UnsetValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
