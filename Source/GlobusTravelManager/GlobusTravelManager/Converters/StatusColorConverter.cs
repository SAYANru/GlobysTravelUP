using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace GlobusTravelManager.Converters
{
    public class StatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string status = value as string;
            if (status == null) return Brushes.Gray;

            return status.ToLower() switch
            {
                "новая" => new SolidColorBrush(Color.FromRgb(255, 165, 0)), // Оранжевый
                "подтверждена" => new SolidColorBrush(Color.FromRgb(76, 175, 80)), // Зеленый
                "отменена" => new SolidColorBrush(Color.FromRgb(244, 67, 54)), // Красный
                "в обработке" => new SolidColorBrush(Color.FromRgb(33, 150, 243)), // Синий
                _ => new SolidColorBrush(Color.FromRgb(158, 158, 158)) // Серый
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}