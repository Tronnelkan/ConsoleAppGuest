﻿// WpfApp/Helpers/PlaceholderVisibilityConverter.cs
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace WpfApp.Helpers
{
    public class PlaceholderVisibilityConverter : IValueConverter
    {
        // Если текст пустой или null, отображаем плейсхолдер
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var text = value as string;
            return string.IsNullOrEmpty(text) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
