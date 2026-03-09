using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using LogAnalyzer.Core.Models;

namespace LogAnalyzer.UI.Converters;

/// <summary>
/// Converts a <see cref="LogLevel"/> to a modern display color.
/// Implements <see cref="IValueConverter"/> for XAML binding.
/// </summary>
[ValueConversion(typeof(LogLevel), typeof(Brush))]
public class LogLevelToColorConverter : IValueConverter
{
    /// <summary>
    /// Converts a LogLevel to a modern Material Design color.
    /// </summary>
    /// <param name="value">The LogLevel value to convert.</param>
    /// <param name="targetType">The target type (should be Brush).</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A Brush representing the color for the specified log level.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LogLevel level)
        {
            return level switch
            {
                LogLevel.Trace => new SolidColorBrush(Color.FromRgb(158, 158, 158)),   // Material Grey
                LogLevel.Debug => new SolidColorBrush(Color.FromRgb(33, 150, 243)),    // Material Blue
                LogLevel.Info => new SolidColorBrush(Color.FromRgb(0, 150, 136)),      // Material Teal
                LogLevel.Warning => new SolidColorBrush(Color.FromRgb(255, 152, 0)),   // Material Orange
                LogLevel.Error => new SolidColorBrush(Color.FromRgb(244, 67, 54)),     // Material Red
                LogLevel.Critical => new SolidColorBrush(Color.FromRgb(183, 28, 28)),  // Material Dark Red
                _ => new SolidColorBrush(Color.FromRgb(33, 33, 33))                    // Dark Grey
            };
        }
        return new SolidColorBrush(Color.FromRgb(33, 33, 33));
    }

    /// <summary>
    /// Back conversion is not supported for this converter.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture to use.</param>
    /// <returns>Throws <see cref="NotImplementedException"/>.</returns>
    /// <exception cref="NotImplementedException">Always thrown as back conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}

/// <summary>
/// Converts a <see cref="LogLevel"/> to an appropriate background highlight color.
/// Implements <see cref="IValueConverter"/> for XAML binding.
/// </summary>
[ValueConversion(typeof(LogLevel), typeof(Brush))]
public class LogLevelToBackgroundConverter : IValueConverter
{
    /// <summary>
    /// Converts a LogLevel to a subtle background highlight color.
    /// </summary>
    /// <param name="value">The LogLevel to convert.</param>
    /// <param name="targetType">The target type (should be Brush).</param>
    /// <param name="parameter">Optional converter parameter.</param>
    /// <param name="culture">The culture to use in the converter.</param>
    /// <returns>A Brush for background highlighting based on log severity.</returns>
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is LogLevel level)
        {
            return level switch
            {
                LogLevel.Warning => new SolidColorBrush(Color.FromRgb(255, 245, 200)), // Light Yellow
                LogLevel.Error => new SolidColorBrush(Color.FromRgb(255, 235, 235)),   // Light Red
                LogLevel.Critical => new SolidColorBrush(Color.FromRgb(255, 220, 220)), // Pinkish
                _ => Brushes.Transparent
            };
        }

        return Brushes.Transparent;
    }

    /// <summary>
    /// Back conversion is not supported for this converter.
    /// </summary>
    /// <param name="value">The value to convert back.</param>
    /// <param name="targetType">The target type.</param>
    /// <param name="parameter">Optional parameter.</param>
    /// <param name="culture">The culture to use.</param>
    /// <returns>Throws <see cref="NotImplementedException"/>.</returns>
    /// <exception cref="NotImplementedException">Always thrown as back conversion is not supported.</exception>
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}