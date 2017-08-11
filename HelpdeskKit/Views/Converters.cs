﻿using System;
using System.Windows;
using System.Windows.Data;

namespace HelpdeskKit.Views
{
    public class InverseBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a boolean");

            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {

            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            bool visible = (bool)value;
            return visible ? Visibility.Visible : Visibility.Hidden;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {

            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");
            if (value == null) throw new NullReferenceException("Value is null");
            var visible = (Visibility)value;
            return visible == Visibility.Visible;

        }
    }

    public class ReverseBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            if (targetType != typeof(Visibility))
                throw new InvalidOperationException("The target must be a Visibility");
            bool visible = (bool)value;
            return visible ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {

            if (targetType != typeof(bool))
                throw new InvalidOperationException("The target must be a bool");
            if (value == null) throw new NullReferenceException("Value is null");
            var visible = (Visibility)value;
            return visible == Visibility.Hidden;


        }
    }
}
